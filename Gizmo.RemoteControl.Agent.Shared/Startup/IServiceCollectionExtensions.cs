using Gizmo.RemoteControl.Shared.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Gizmo.RemoteControl.Agent.Shared.Services;

namespace Gizmo.RemoteControl.Agent.Shared.Startup;

public static class IServiceCollectionExtensions
{
    internal static void AddRemoteControlXplat(
        this IServiceCollection services,
        Action<IRemoteControlClientBuilder> clientConfig)
    {
        var builder = new RemoteControlClientBuilder(services);
        clientConfig.Invoke(builder);
        builder.Validate();

        services.AddLogging(builder =>
        {
            builder.AddConsole().AddDebug();
        });

        services.AddSingleton<ISystemTime, SystemTime>();
        services.AddSingleton<IDesktopHubConnection, DesktopHubConnection>();
        services.AddSingleton<IIdleTimer, IdleTimer>();
        services.AddSingleton<IImageHelper, ImageHelper>();
        services.AddSingleton<IChatHostService, ChatHostService>();
        //services.AddSingleton(s => WeakReferenceMessenger.Default); // TODO: Remove
        services.AddSingleton<IEnvironmentHelper, EnvironmentHelper>();
        services.AddSingleton<IDtoMessageHandler, DtoMessageHandler>();
        services.AddSingleton<IAppState, AppState>();
        services.AddSingleton<IViewerFactory, ViewerFactory>();
        services.AddTransient<IScreenCaster, ScreenCaster>();
        services.AddTransient<IHubConnectionBuilder>(s => new HubConnectionBuilder());
    }
}
