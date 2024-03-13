using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Gizmo.RemoteControl.Desktop.Windows.Helpers
{
    public static partial class ClipboardHelper
    {
        private const uint CF_UNICODETEXT = 13;

        public static string? GetText(CancellationToken cancellationToken = default)
        {
            if (!IsClipboardFormatAvailable(CF_UNICODETEXT))
            {
                return null;
            }

            TryOpenClipboard(cancellationToken);

            return InnerGet();
        }

        public static void SetText(string text, CancellationToken cancellationToken = default)
        {
            TryOpenClipboard(cancellationToken);

            InnerSet(text);
        }

        static void InnerSet(string text)
        {
            EmptyClipboard();
            IntPtr hGlobal = default;
            try
            {
                var bytes = (text.Length + 1) * 2;
                hGlobal = Marshal.AllocHGlobal(bytes);

                if (hGlobal == default)
                {
                    ThrowLastWin32Exception();
                }

                var target = GlobalLock(hGlobal);

                if (target == default)
                {
                    ThrowLastWin32Exception();
                }

                try
                {
                    Marshal.Copy(text.ToCharArray(), 0, target, text.Length);
                }
                finally
                {
                    GlobalUnlock(target);
                }

                if (SetClipboardData(CF_UNICODETEXT, hGlobal) == default)
                {
                    ThrowLastWin32Exception();
                }

                hGlobal = default;
            }
            finally
            {
                if (hGlobal != default)
                {
                    Marshal.FreeHGlobal(hGlobal);
                }

                CloseClipboard();
            }
        }

        static unsafe string? InnerGet()
        {
            IntPtr handle = default;

            IntPtr pointer = default;
            try
            {
                handle = GetClipboardData(CF_UNICODETEXT);
                if (handle == default)
                {
                    return null;
                }

                pointer = GlobalLock(handle);
                if (pointer == default)
                {
                    return null;
                }

                var size = GlobalSize(handle);
                var span = new Span<byte>((void*)handle, size);
                return Encoding.Unicode.GetString(span).TrimEnd('\0');
            }
            finally
            {
                if (pointer != default)
                {
                    GlobalUnlock(handle);
                }

                CloseClipboard();
            }
        }

        static void TryOpenClipboard(CancellationToken cancellationToken = default)
        {
            var num = 10;
            while (true)
            {
                if (OpenClipboard(default))
                {
                    break;
                }

                if (--num == 0)
                {
                    ThrowLastWin32Exception();
                }

                Thread.Sleep(100);
            }
        }

        static void ThrowLastWin32Exception()
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool IsClipboardFormatAvailable(uint format);

        [LibraryImport("user32.dll", SetLastError = true)]
        private static partial IntPtr GetClipboardData(uint uFormat);

        [LibraryImport("user32.dll", SetLastError = true)]
        private static partial IntPtr SetClipboardData(uint uFormat, IntPtr data);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        private static partial IntPtr GlobalLock(IntPtr hMem);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool GlobalUnlock(IntPtr hMem);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool OpenClipboard(IntPtr hWndNewOwner);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool CloseClipboard();

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool EmptyClipboard();

        [LibraryImport("Kernel32.dll", SetLastError = true)]
        private static partial int GlobalSize(IntPtr hMem);
    }
}
