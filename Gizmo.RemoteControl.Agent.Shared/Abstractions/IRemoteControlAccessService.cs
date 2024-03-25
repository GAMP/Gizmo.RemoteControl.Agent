using Gizmo.RemoteControl.Shared.Enums;

namespace Gizmo.RemoteControl.Agent.Shared.Abstractions;

public interface IRemoteControlAccessService
{
    bool IsPromptOpen { get; }

    Task<PromptForAccessResult> PromptForAccess(string requesterName, string organizationName);
}
