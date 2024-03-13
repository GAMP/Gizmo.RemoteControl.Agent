using Gizmo.RemoteControl.Desktop.Shared.Abstractions;
using Gizmo.RemoteControl.Desktop.Shared.Services;
using Gizmo.RemoteControl.Desktop.Shared.ViewModels;

namespace Gizmo.RemoteControl.Desktop.Windows.Services;

public class FileTransferService : IFileTransferService
{
    public string GetBaseDirectory()
    {
        throw new NotImplementedException();
    }

    public void OpenFileTransferWindow(IViewer viewer)
    {
        throw new NotImplementedException();
    }

    public Task ReceiveFile(byte[] buffer, string fileName, string messageId, bool endOfFile, bool startOfFile)
    {
        throw new NotImplementedException();
    }

    public Task UploadFile(FileUpload file, IViewer viewer, Action<double> progressUpdateCallback, CancellationToken cancelToken)
    {
        throw new NotImplementedException();
    }
}
