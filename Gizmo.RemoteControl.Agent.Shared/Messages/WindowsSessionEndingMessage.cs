using Gizmo.RemoteControl.Shared.Enums;

namespace Gizmo.RemoteControl.Desktop.Shared.Messages;

public class WindowsSessionEndingMessage
{
    public WindowsSessionEndingMessage(SessionEndReasonsEx reason)
    {
        Reason = reason;
    }

    public SessionEndReasonsEx Reason { get; }
}
