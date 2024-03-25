using Gizmo.RemoteControl.Agent.Shared.Abstractions;
using Gizmo.RemoteControl.Agent.Shared.Services;
using Gizmo.RemoteControl.Agent.Shared.ViewModels;

namespace Gizmo.RemoteControl.Agent.Windows.Services;

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
