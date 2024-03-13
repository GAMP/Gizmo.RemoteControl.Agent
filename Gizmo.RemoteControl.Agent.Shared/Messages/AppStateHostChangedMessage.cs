﻿namespace Gizmo.RemoteControl.Desktop.Shared.Messages;

public class AppStateHostChangedMessage
{
    public AppStateHostChangedMessage(string newHost)
    {
        NewHost = newHost;
    }

    public string NewHost { get; }
}
