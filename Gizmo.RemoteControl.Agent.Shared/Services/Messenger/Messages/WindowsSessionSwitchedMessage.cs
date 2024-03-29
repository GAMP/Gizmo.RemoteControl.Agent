﻿using Gizmo.RemoteControl.Shared.Enums;

namespace Gizmo.RemoteControl.Agent.Shared.Services.Messenger.Messages;

public class WindowsSessionSwitchedMessage
{
    public WindowsSessionSwitchedMessage(SessionSwitchReasonEx reason, int sessionId)
    {
        Reason = reason;
        SessionId = sessionId;
    }

    public SessionSwitchReasonEx Reason { get; }
    public int SessionId { get; }
}
