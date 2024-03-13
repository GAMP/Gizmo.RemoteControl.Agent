using Gizmo.RemoteControl.Desktop.Shared.Native.Linux;
using System.Security.Principal;

namespace Gizmo.RemoteControl.Desktop.Shared.Services;

/// <summary>
/// Environment parameters helper.
/// </summary>
public interface IEnvironmentHelper
{
    /// <summary>
    /// Indicates that build was done in debug mode.
    /// </summary>
    bool IsDebug { get; }

    /// <summary>
    /// Indicates that application is running elevated.
    /// </summary>
    bool IsElevated { get; }
}

public sealed class EnvironmentHelper : IEnvironmentHelper
{
    public bool IsDebug
    {
        get
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }

    public bool IsElevated
    {
        get
        {
            if (OperatingSystem.IsWindows())
            {
                using var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            if (OperatingSystem.IsLinux())
            {
                return Libc.geteuid() == 0;
            }
            return false;
        }
    }
}
