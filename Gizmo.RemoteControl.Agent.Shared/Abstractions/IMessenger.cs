using Gizmo.RemoteControl.Agent.Shared.Messages;

namespace Gizmo.RemoteControl.Agent.Shared.Abstractions
{
    public interface IMessenger
    {
        Task<IAsyncDisposable> Register<T>(object _, Func<T, Task> handle) where T : class;
        void Send(AppStateHostChangedMessage message);
        void Send(WindowsSessionSwitchedMessage message);
        void Send(WindowsSessionEndingMessage message);
        void Send(DisplaySettingsChangedMessage message);
    }
}
