namespace Gizmo.RemoteControl.Agent.Shared.Services.Messenger.Messages;

public class AppStateHostChangedMessage
{
    public AppStateHostChangedMessage(string newHost)
    {
        NewHost = newHost;
    }

    public string NewHost { get; }
}
