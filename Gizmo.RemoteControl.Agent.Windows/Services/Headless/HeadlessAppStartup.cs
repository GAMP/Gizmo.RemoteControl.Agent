using Gizmo.RemoteControl.Agent.Shared.Abstractions;
using Gizmo.RemoteControl.Agent.Shared.Enums;
using Gizmo.RemoteControl.Agent.Shared.Native.Windows;
using Gizmo.RemoteControl.Agent.Shared.Services;
using Gizmo.RemoteControl.Shared.Models;

using Microsoft.Extensions.Logging;

namespace Gizmo.RemoteControl.Agent.Windows.Services.Headless;

internal class HeadlessAppStartup : IAppStartup
{
    private readonly IAppState _appState;
    private readonly IKeyboardMouseInput _inputService;
    private readonly IDesktopHubConnection _desktopHub;
    private readonly IClipboardService _clipboardService;
    private readonly IChatHostService _chatHostService;
    private readonly ICursorIconWatcher _cursorIconWatcher;
    private readonly IMessageLoop _messageLoop;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IIdleTimer _idleTimer;
    private readonly IShutdownService _shutdownService;
    private readonly IBrandingProvider _brandingProvider;
    private readonly ILogger<HeadlessAppStartup> _logger;

    public HeadlessAppStartup(
        IAppState appState,
        IKeyboardMouseInput inputService,
        IDesktopHubConnection desktopHub,
        IClipboardService clipboardService,
        IChatHostService chatHostService,
        ICursorIconWatcher iconWatcher,
        IMessageLoop messageLoop,
        IHostApplicationLifetime applicationLifetime,
        IIdleTimer idleTimer,
        IShutdownService shutdownService,
        IBrandingProvider brandingProvider,
        ILogger<HeadlessAppStartup> logger)
    {
        _appState = appState;
        _inputService = inputService;
        _desktopHub = desktopHub;
        _clipboardService = clipboardService;
        _chatHostService = chatHostService;
        _cursorIconWatcher = iconWatcher;
        _messageLoop = messageLoop;
        _applicationLifetime = applicationLifetime;
        _idleTimer = idleTimer;
        _shutdownService = shutdownService;
        _brandingProvider = brandingProvider;
        _logger = logger;
    }

    public async Task Run()
    {
        await _brandingProvider.Initialize();

        _messageLoop.StartMessageLoop();


        if (_appState.Mode is AppMode.Unattended or AppMode.Attended)
        {
            _clipboardService.BeginWatching();
            _inputService.Init();
            _cursorIconWatcher.OnChange += CursorIconWatcher_OnChange;
        }

        switch (_appState.Mode)
        {
            case AppMode.Unattended:
                {
                    await StartScreenCasting().ConfigureAwait(false);
                    break;
                }
            case AppMode.Attended:
            case AppMode.Chat:
                throw new InvalidOperationException("Only unattended mode is supported.");
            default:
                break;
        }
    }


    private async Task StartScreenCasting()
    {
        if (!await _desktopHub.Connect(TimeSpan.FromSeconds(30), _applicationLifetime.ApplicationStopping))
        {
            await _shutdownService.Shutdown();
            return;
        }

        var result = await _desktopHub.SendUnattendedSessionInfo(
                 _appState.SessionId,
                 _appState.AccessKey,
                 Environment.MachineName,
                 _appState.RequesterName,
                 _appState.OrganizationName);

        if (!result.IsSuccess)
        {
            _logger.LogError(result.Exception, "An error occurred while trying to establish a session with the server.");
            await _shutdownService.Shutdown();
            return;
        }

        try
        {
            if (Win32Interop.GetCurrentDesktop(out var currentDesktopName))
            {
                _logger.LogInformation("Setting initial desktop to {currentDesktopName}.", currentDesktopName);
            }
            else
            {
                _logger.LogWarning("Failed to get initial desktop name.");
            }

            if (!Win32Interop.SwitchToInputDesktop())
            {
                _logger.LogWarning("Failed to set initial desktop.");
            }

            if (_appState.IsRelaunch)
            {
                _logger.LogInformation("Resuming after relaunch.");
                var viewerIDs = _appState.RelaunchViewers;
                await _desktopHub.NotifyViewersRelaunchedScreenCasterReady(viewerIDs);
            }
            else
            {
                await _desktopHub.NotifyRequesterUnattendedReady();
            }
        }
        finally
        {
            _idleTimer.Start();
        }
    }

    private async void CursorIconWatcher_OnChange(object? sender, CursorInfo cursor)
    {
        if (_appState.Viewers.Any() == true &&
            _desktopHub.IsConnected)
        {
            foreach (var viewer in _appState.Viewers.Values)
            {
                await viewer.SendCursorChange(cursor);
            }
        }
    }
}
