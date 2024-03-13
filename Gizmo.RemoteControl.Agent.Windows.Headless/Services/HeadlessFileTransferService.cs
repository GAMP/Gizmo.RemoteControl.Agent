using Gizmo.RemoteControl.Desktop.Shared.Abstractions;
using Gizmo.RemoteControl.Desktop.Shared.Services;
using Gizmo.RemoteControl.Desktop.Shared.ViewModels;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Gizmo.RemoteControl.Desktop.Windows.Services;

public class HeadlessFileTransferService : IFileTransferService
{

    private static readonly ConcurrentDictionary<string, FileStream> _partialTransfers =
        new();

    private static readonly SemaphoreSlim _writeLock = new(1, 1);
 
    private readonly ILogger<HeadlessFileTransferService> _logger;

    public HeadlessFileTransferService(
        ILogger<HeadlessFileTransferService> logger)
    {
        _logger = logger;
    }

    public string GetBaseDirectory()
    {
        var programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        return Directory.CreateDirectory(Path.Combine(programDataPath, "RemoteControl", "Shared")).FullName;
    }

    public void OpenFileTransferWindow(IViewer viewer)
    {
    }

    [SupportedOSPlatform("windows")]
    public async Task ReceiveFile(byte[] buffer, string fileName, string messageId, bool endOfFile, bool startOfFile)
    {
        try
        {
            await _writeLock.WaitAsync();

            var baseDir = GetBaseDirectory();

            SetFileOrFolderPermissions(baseDir);

            if (startOfFile)
            {
                var filePath = Path.Combine(baseDir, fileName);

                if (File.Exists(filePath))
                {
                    var count = 0;
                    var ext = Path.GetExtension(fileName);
                    var fileWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    while (File.Exists(filePath))
                    {
                        filePath = Path.Combine(baseDir, $"{fileWithoutExt}-{count}{ext}");
                        count++;
                    }
                }

                File.Create(filePath).Close();
                SetFileOrFolderPermissions(filePath);
                var fs = new FileStream(filePath, FileMode.OpenOrCreate);
                _partialTransfers.AddOrUpdate(messageId, fs, (k, v) => fs);
            }

            var fileStream = _partialTransfers[messageId];

            if (buffer?.Length > 0)
            {
                await fileStream.WriteAsync(buffer);

            }

            if (endOfFile)
            {
                fileStream.Close();
                _partialTransfers.Remove(messageId, out _);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while receiving file.");
        }
        finally
        {
            _writeLock.Release();
            if (endOfFile)
            {
                await ShowTransferComplete();
            }
        }
    }

    public async Task UploadFile(
        FileUpload fileUpload,
        IViewer viewer,
        Action<double> progressUpdateCallback,
        CancellationToken cancelToken)
    {
        try
        {
            await viewer.SendFile(fileUpload, progressUpdateCallback, cancelToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while uploading file.");
        }
    }

    [SupportedOSPlatform("windows")]
    private static void SetFileOrFolderPermissions(string path)
    {
        FileSystemSecurity ds;

        var aclSections = AccessControlSections.Access | AccessControlSections.Group | AccessControlSections.Owner;
        if (File.Exists(path))
        {
            ds = new FileSecurity(path, aclSections);
        }
        else if (Directory.Exists(path))
        {
            ds = new DirectorySecurity(path, aclSections);
        }
        else
        {
            return;
        }

        var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
        var account = (NTAccount)sid.Translate(typeof(NTAccount));

        var accessAlreadySet = false;

        foreach (FileSystemAccessRule rule in ds.GetAccessRules(true, true, typeof(NTAccount)))
        {
            if (rule.IdentityReference == account &&
                rule.FileSystemRights.HasFlag(FileSystemRights.Modify) &&
                rule.AccessControlType == AccessControlType.Allow)
            {
                accessAlreadySet = true;
                break;
            }
        }

        if (!accessAlreadySet)
        {
            ds.AddAccessRule(new FileSystemAccessRule(account, FileSystemRights.Modify, AccessControlType.Allow));
            if (File.Exists(path))
            {
                new FileInfo(path).SetAccessControl((FileSecurity)ds);
            }
            else if (Directory.Exists(path))
            {
                new DirectoryInfo(path).SetAccessControl((DirectorySecurity)ds);
            }
        }
    }

    private async Task ShowTransferComplete()
    {
    }
}
