using Microsoft.Extensions.Hosting;

namespace Agent.Headless.Services;
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
