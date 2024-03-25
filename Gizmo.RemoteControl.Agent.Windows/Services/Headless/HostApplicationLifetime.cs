namespace Gizmo.RemoteControl.Agent.Windows.Services.Headless;
internal class HostApplicationLifetime : IHostApplicationLifetime
{
    public CancellationToken ApplicationStopped => CancellationToken.None;

    public CancellationToken ApplicationStopping => CancellationToken.None;

    public CancellationToken ApplicationStarted => CancellationToken.None;

    public void Shutdown()
    {
        throw new NotImplementedException();
    }
}
