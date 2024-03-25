using Gizmo.RemoteControl.Shared.Models;

namespace Gizmo.RemoteControl.Agent.Shared.Abstractions;

public interface IBrandingProvider
{
    BrandingInfoBase CurrentBranding { get; }
    Task Initialize();
    void SetBrandingInfo(BrandingInfoBase brandingInfo);
}
