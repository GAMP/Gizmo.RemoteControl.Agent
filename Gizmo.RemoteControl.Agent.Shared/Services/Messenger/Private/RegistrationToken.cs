namespace Gizmo.RemoteControl.Agent.Shared.Services.Messenger.Private
{
    class RegistrationToken(Action disposalAction) : IDisposable
    {
        private readonly Action _disposalAction = disposalAction;
        private bool _disposedValue;

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        _disposalAction();
                    }
                    catch
                    {
                        // Ignore errors.
                    }
                }
                _disposedValue = true;
            }
        }
    }
}
