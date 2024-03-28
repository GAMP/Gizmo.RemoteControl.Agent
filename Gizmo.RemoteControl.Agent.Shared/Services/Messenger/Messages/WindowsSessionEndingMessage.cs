using Gizmo.RemoteControl.Shared.Enums;

namespace Gizmo.RemoteControl.Agent.Shared.Services.Messenger.Messages;

public class WindowsSessionEndingMessage
{
    public WindowsSessionEndingMessage(SessionEndReasonsEx reason)
    {
        Reason = reason;
    }

    public SessionEndReasonsEx Reason { get; }
}
