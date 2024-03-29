using Gizmo.RemoteControl.Agent.Shared.Abstractions;
using Gizmo.RemoteControl.Agent.Shared.Services;
using Gizmo.RemoteControl.Shared.Extensions;

using Microsoft.Extensions.Logging;

namespace Gizmo.RemoteControl.Agent.Windows.Services;

public class ShutdownService : IShutdownService
{
    private readonly IDesktopHubConnection _hubConnection;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IAppState _appState;
    private readonly ILogger<ShutdownService> _logger;
    private readonly SemaphoreSlim _shutdownLock = new(1, 1);

    public ShutdownService(
        IDesktopHubConnection hubConnection,
        IHostApplicationLifetime applicationLifetime,
        IAppState appState,
        ILogger<ShutdownService> logger)
    {
        _hubConnection = hubConnection;
        _applicationLifetime = applicationLifetime;
        _appState = appState;
        _logger = logger;
    }

    public async Task Shutdown()
    {
        using var _ = _logger.Enter(LogLevel.Information);

        try
        {
            if (!await _shutdownLock.WaitAsync(0))
            {
                // We've made our best effort to shutdown gracefully, but WPF will
                // sometimes hang indefinitely.  In that case, we'll forcefully close.
                _logger.LogInformation(
                    "Shutdown was called more than once. Forcing process exit.");
                Environment.FailFast("Process hung during shutdown. Forcefully quitting on second call.");
                return;
            }

            _logger.LogInformation("Starting process shutdown.");

            _logger.LogInformation("Disconnecting viewers.");
            await TryDisconnectViewers();

            _logger.LogInformation("Shutting down UI dispatchers.");
            _applicationLifetime.Shutdown();

            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while shutting down.");
            Environment.Exit(1);
        }
        finally
        {
            _shutdownLock.Release();
        }
    }

    private async Task TryDisconnectViewers()
    {
        try
        {
            if (_hubConnection.IsConnected && _appState.Viewers.Any())
            {
                await _hubConnection.DisconnectAllViewers();
                await _hubConnection.Disconnect();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending shutdown notice to viewers.");
        }
    }
}
