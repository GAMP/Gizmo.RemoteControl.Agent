﻿using Gizmo.RemoteControl.Agent.Shared.Abstractions;
using Gizmo.RemoteControl.Agent.Shared.Services.Messenger.Messages;
using Gizmo.RemoteControl.Shared.Enums;

using Microsoft.Extensions.Logging;
using Microsoft.Win32;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Gizmo.RemoteControl.Agent.Windows.Services;

public interface IMessageLoop
{
    void StartMessageLoop();
}

[SupportedOSPlatform("windows")]
public class MessageLoop : IMessageLoop
{
    private readonly CancellationToken _exitToken;
    private readonly ILogger<MessageLoop> _logger;
    private readonly IMessenger _messenger;
    private Thread? _messageLoopThread;

    public MessageLoop(
        IMessenger messenger,
        IHostApplicationLifetime applicationLifetime,
        ILogger<MessageLoop> logger)
    {
        _messenger = messenger;
        _logger = logger;
        _exitToken = applicationLifetime.ApplicationStopping;
    }


    public void StartMessageLoop()
    {
        if (_messageLoopThread is not null)
        {
            throw new InvalidOperationException("Message loop already started.");
        }

        _messageLoopThread = new Thread(() =>
        {
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            SystemEvents.SessionEnding += SystemEvents_SessionEnding;
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            while (!_exitToken.IsCancellationRequested)
            {
                try
                {
                    while (GetMessage(out var msg, nint.Zero, 0, 0) > 0)
                    {
                        DispatchMessage(ref msg);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in message loop.");
                }
            }

            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
            SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
            SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
        });
        _messageLoopThread.SetApartmentState(ApartmentState.STA);
        _messageLoopThread.Start();
    }

    [DllImport("user32.dll")]
    private static extern bool DispatchMessage([In] ref MSG lpmsg);

    [DllImport("user32.dll")]
    private static extern int GetMessage(out MSG lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);


    private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
    {
        _messenger.Send(new DisplaySettingsChangedMessage());
    }

    private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
    {
        _logger.LogInformation("Session ending.  Reason: {reason}", e.Reason);

        var reason = (SessionEndReasonsEx)e.Reason;
        _messenger.Send(new WindowsSessionEndingMessage(reason));
    }

    private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        _logger.LogInformation("Session changing.  Reason: {reason}", e.Reason);

        var reason = (SessionSwitchReasonEx)(int)e.Reason;
        _messenger.Send(new WindowsSessionSwitchedMessage(reason, Process.GetCurrentProcess().SessionId));
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct MSG
    {
        public nint hwnd;
        public uint message;
        public nint wParam;
        public nint lParam;
        public uint time;
        public POINT pt;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
