namespace Gizmo.RemoteControl.Agent.Windows;

public interface IHostApplicationLifetime
{
    public CancellationToken ApplicationStopped { get; }

    public CancellationToken ApplicationStopping { get; }

    public CancellationToken ApplicationStarted { get; }

    void Shutdown();
}
