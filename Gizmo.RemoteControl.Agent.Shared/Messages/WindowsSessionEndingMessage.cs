using Gizmo.RemoteControl.Shared.Enums;

namespace Gizmo.RemoteControl.Agent.Shared.Messages;

public class WindowsSessionEndingMessage
{
    public WindowsSessionEndingMessage(SessionEndReasonsEx reason)
    {
        Reason = reason;
    }

    public SessionEndReasonsEx Reason { get; }
}
