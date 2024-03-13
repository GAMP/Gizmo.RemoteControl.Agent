using Gizmo.RemoteControl.Desktop.Shared.Abstractions;
using Gizmo.RemoteControl.Desktop.Windows.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Agent.Headless.Services;

public class ClipboardService : IClipboardService
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<ClipboardService> _logger;
    private Task? _watcherTask;

    public event EventHandler<string>? ClipboardTextChanged;

    public ClipboardService(
        IHostApplicationLifetime applicationLifetime,
        ILogger<ClipboardService> logger)
    {
        _applicationLifetime = applicationLifetime;
        _logger = logger;
    }

    private string ClipboardText { get; set; } = string.Empty;

    public void BeginWatching()
    {
        if (_watcherTask?.Status == TaskStatus.Running)
        {
            return;
        }

        _watcherTask = Task.Run(
            async () => await WatchClipboard(_applicationLifetime.ApplicationStopping),
            _applicationLifetime.ApplicationStopping);
    }

    public Task SetText(string clipboardText)
    {
        try
        {
            ClipboardHelper.SetText(clipboardText);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while setting text.");
        }

        return Task.CompletedTask;
    }

    private async Task WatchClipboard(CancellationToken cancelToken)
    {
        while (
            !cancelToken.IsCancellationRequested &&
            !Environment.HasShutdownStarted)
        {
            try
            {
                var currentText = ClipboardHelper.GetText(cancelToken);
                if (!string.IsNullOrEmpty(currentText) && currentText != ClipboardText)
                {
                    ClipboardText = currentText;
                    ClipboardTextChanged?.Invoke(this, ClipboardText);
                }
            }
            finally
            {
                await Task.Delay(500, cancelToken);
            }
        }
    }
}
