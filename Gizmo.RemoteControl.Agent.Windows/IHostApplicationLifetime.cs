namespace Microsoft.Extensions.Hosting;

public interface IHostApplicationLifetime
{
    public CancellationToken ApplicationStopped { get; }

    public CancellationToken ApplicationStopping { get; }

    public CancellationToken ApplicationStarted { get; }

    void Shutdown();
}
