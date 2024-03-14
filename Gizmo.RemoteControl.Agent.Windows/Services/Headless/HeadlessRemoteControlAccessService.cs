using Gizmo.RemoteControl.Desktop.Shared.Abstractions;
using Gizmo.RemoteControl.Shared.Enums;

using Microsoft.Extensions.Logging;

namespace Gizmo.RemoteControl.Agent.Windows.Services.Headless;

public class HeadlessRemoteControlAccessService : IRemoteControlAccessService
{
    private readonly ILogger<HeadlessRemoteControlAccessService> _logger;

    public HeadlessRemoteControlAccessService(
        ILogger<HeadlessRemoteControlAccessService> logger)
    {
        _logger = logger;
    }

    public bool IsPromptOpen => false;

    public Task<PromptForAccessResult> PromptForAccess(string requesterName, string organizationName)
    {
        return Task.FromResult(PromptForAccessResult.Accepted);
    }
}
