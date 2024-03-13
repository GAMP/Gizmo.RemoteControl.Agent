using Gizmo.RemoteControl.Desktop.Shared.Abstractions;
using Gizmo.RemoteControl.Shared.Models;

namespace Gizmo.RemoteControl.Desktop.Windows.Services;

public class HeadlessChatUIService : IChatUiService
{
    public HeadlessChatUIService()
    {
    }

    public event EventHandler? ChatWindowClosed;

    public Task ReceiveChat(ChatMessage chatMessage)
    {
        return Task.CompletedTask;
    }

    public void ShowChatWindow(string organizationName, StreamWriter writer)
    {
    }

}
