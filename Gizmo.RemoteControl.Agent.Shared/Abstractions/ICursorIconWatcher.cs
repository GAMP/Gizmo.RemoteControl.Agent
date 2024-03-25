using Gizmo.RemoteControl.Shared.Models;

namespace Gizmo.RemoteControl.Agent.Shared.Abstractions;

public interface ICursorIconWatcher
{
    [Obsolete("This should be replaced with a message published by IMessenger.")]
    event EventHandler<CursorInfo> OnChange;

    CursorInfo GetCurrentCursor();
}
