﻿using Gizmo.RemoteControl.Shared;
using Gizmo.RemoteControl.Shared.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Gizmo.RemoteControl.Agent.Windows.Services.Headless;
using Gizmo.RemoteControl.Agent.Windows.Services;
using Gizmo.RemoteControl.Agent.Shared.Services;
using Gizmo.RemoteControl.Agent.Shared.Abstractions;
using Gizmo.RemoteControl.Agent.Shared;
using Gizmo.RemoteControl.Agent.Shared.Enums;
using Gizmo.RemoteControl.Agent.Windows.Services.Messenger;

namespace Gizmo.RemoteControl.Agent.Windows;

[SupportedOSPlatform("windows")]
public static class Extensions
{
    /// <summary>
    /// Adds generic remote control services.
    /// </summary>
    /// <param name="services">Service collection.</param>
    public static void AddRemoteControlServices(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddConsole().AddDebug();
        });

        services.AddSingleton<ISystemTime, SystemTime>();
        services.AddSingleton<IDesktopHubConnection, DesktopHubConnection>();
        services.AddSingleton<IIdleTimer, IdleTimer>();
        services.AddSingleton<IImageHelper, ImageHelper>();
        services.AddSingleton<IChatHostService, ChatHostService>();
        services.AddSingleton<IMessenger, MessengerService>();
        services.AddSingleton<IDtoMessageHandler, DtoMessageHandler>();
        services.AddTransient<IHubConnectionBuilder>(s => new HubConnectionBuilder());
        services.AddSingleton<IAppState, AppState>();
        services.AddSingleton<IViewerFactory, ViewerFactory>();
        services.AddSingleton<IEnvironmentHelper, EnvironmentHelper>();
        services.AddTransient<IScreenCaster, ScreenCaster>();

        services.AddSingleton<IHostApplicationLifetime, HostApplicationLifetime>();
        services.AddSingleton<IClipboardService, ClipboardService>();
        services.AddSingleton<IAppStartup, HeadlessAppStartup>();

        services.AddSingleton<ICursorIconWatcher, CursorIconWatcher>();
        services.AddSingleton<IKeyboardMouseInput, KeyboardMouseInput>();
        services.AddSingleton<IAudioCapturer, AudioCapturer>();
        services.AddSingleton<IShutdownService, ShutdownService>();
        services.AddSingleton<IMessageLoop, MessageLoop>();
        services.AddTransient<IRemoteControlAccessService, HeadlessRemoteControlAccessService>(); //custom
        services.AddTransient<IFileTransferService, HeadlessFileTransferService>(); //custom
        services.AddTransient<IChatUiService, HeadlessChatUIService>(); //custom
        services.AddTransient<IBrandingProvider, BrandingProvider>(); //custom
        services.AddTransient<ISessionIndicator, HeadlessSessionIndicator>(); //custom
        services.AddTransient<IScreenCapturer, ScreenCapturer>();
    }

    public static async Task<Result> UseHeadlessClient(this IServiceProvider services,
        string host, string sessionId, string accessKey)
    {
        StaticServiceProvider.Instance = services;

        var appState = services.GetRequiredService<IAppState>();

        appState.Configure(host,
            AppMode.Unattended,
            sessionId,
            accessKey,
            string.Empty,
            string.Empty,
            string.Empty,
            false,
            string.Empty,
            false);

        var appStartup = services.GetRequiredService<IAppStartup>();
        await appStartup.Run();
        return Result.Ok();
    }
}
