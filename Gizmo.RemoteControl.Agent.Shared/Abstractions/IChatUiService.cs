using Gizmo.RemoteControl.Shared.Models;

namespace Gizmo.RemoteControl.Desktop.Shared.Abstractions;

public interface IChatUiService
{
    event EventHandler ChatWindowClosed;

    void ShowChatWindow(string organizationName, StreamWriter writer);
    Task ReceiveChat(ChatMessage chatMessage);
}
