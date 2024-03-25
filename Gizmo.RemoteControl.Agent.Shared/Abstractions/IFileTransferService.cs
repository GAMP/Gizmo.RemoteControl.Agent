using Gizmo.RemoteControl.Agent.Shared.Services;
using Gizmo.RemoteControl.Agent.Shared.ViewModels;

namespace Gizmo.RemoteControl.Agent.Shared.Abstractions;

public interface IFileTransferService
{
    string GetBaseDirectory();

    Task ReceiveFile(byte[] buffer, string fileName, string messageId, bool endOfFile, bool startOfFile);
    void OpenFileTransferWindow(IViewer viewer);
    Task UploadFile(FileUpload file, IViewer viewer, Action<double> progressUpdateCallback, CancellationToken cancelToken);
}
