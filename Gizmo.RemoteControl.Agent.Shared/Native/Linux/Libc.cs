using System.Runtime.InteropServices;

namespace Gizmo.RemoteControl.Desktop.Shared.Native.Linux;

public class Libc
{
    [DllImport("libc", SetLastError = true)]
    public static extern uint geteuid();
}
