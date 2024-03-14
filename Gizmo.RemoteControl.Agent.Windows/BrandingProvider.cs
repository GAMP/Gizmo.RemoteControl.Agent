using Gizmo.RemoteControl.Desktop.Shared.Abstractions;
using Gizmo.RemoteControl.Shared.Models;

namespace Gizmo.RemoteControl.Agent.Windows;

internal class BrandingProvider : IBrandingProvider
{
    public BrandingInfoBase CurrentBranding => new() { Icon = Array.Empty<byte>() };

    public Task<BrandingInfoBase> GetBrandingInfo()
    {
        return Task.FromResult(CurrentBranding);
    }

    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    public void SetBrandingInfo(BrandingInfoBase brandingInfo)
    {
    }
}
