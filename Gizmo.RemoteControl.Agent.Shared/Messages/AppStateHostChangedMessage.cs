﻿namespace Gizmo.RemoteControl.Agent.Shared.Messages;

public class AppStateHostChangedMessage
{
    public AppStateHostChangedMessage(string newHost)
    {
        NewHost = newHost;
    }

    public string NewHost { get; }
}
