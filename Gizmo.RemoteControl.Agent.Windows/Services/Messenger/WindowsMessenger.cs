using System.Collections.Concurrent;
using System.Collections.Immutable;

using Gizmo.RemoteControl.Agent.Shared.Abstractions;

namespace Gizmo.RemoteControl.Agent.Windows.Services.Messenger
{
    class RegistrationToken : IDisposable
    {
        private readonly Action _disposalAction;
        private bool _disposedValue;

        public RegistrationToken(Action disposalAction)
        {
            _disposalAction = disposalAction;
        }

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

    public sealed class WindowsMessenger : IMessenger
    {
        private readonly SemaphoreSlim _registrationLock = new(1, 1);
        private readonly ConcurrentDictionary<CompositeKey, ConcurrentDictionary<object, WeakReferenceTable>> _subscribers = new();

        /// <inheritdoc />
        public bool IsRegistered<TMessage>(object subscriber)
            where TMessage : class
        {
            return IsRegistered<TMessage, DefaultChannel>(subscriber, DefaultChannel.Instance);
        }

        /// <inheritdoc />
        public bool IsRegistered<TMessage, TChannel>(object subscriber, TChannel channel)
            where TMessage : class
            where TChannel : IEquatable<TChannel>
        {
            _registrationLock.Wait();
            try
            {
                var table = GetWeakReferenceTable<TMessage, TChannel>(channel);

                return table.TryGetValue(subscriber, out _);
            }
            finally
            {
                _registrationLock.Release();
            }
        }

        /// <inheritdoc />
        public IDisposable Register<TMessage>(object subscriber, RegistrationCallback<TMessage> callback)
            where TMessage : class
        {
            return Register(subscriber, DefaultChannel.Instance, callback);
        }

        /// <inheritdoc />
        public IDisposable Register<TMessage, TChannel>(object subscriber, TChannel channel, RegistrationCallback<TMessage> callback)
            where TMessage : class
            where TChannel : IEquatable<TChannel>
        {
            _registrationLock.Wait();
            try
            {
                var table = GetWeakReferenceTable<TMessage, TChannel>(channel);

                if (table.TryGetValue(subscriber, out _))
                {
                    throw new InvalidOperationException(
                        "Subscriber is already registered to the specified message and channel.");
                }

                table.AddOrUpdate(subscriber, callback);

                return new RegistrationToken(() =>
                {
                    Unregister<TMessage, TChannel>(subscriber, channel);
                });
            }
            finally
            {
                _registrationLock.Release();
            }
        }

        /// <inheritdoc />
        public Task<IImmutableList<Exception>> Send<TMessage>(TMessage message)
            where TMessage : class
        {
            return Send(message, DefaultChannel.Instance);
        }

        /// <inheritdoc />
        public async Task<IImmutableList<Exception>> Send<TMessage, TChannel>(TMessage message, TChannel channel)
            where TMessage : class
            where TChannel : IEquatable<TChannel>
        {
            var subscribers = await GetSubscribers<TMessage, TChannel>(channel);
            var exceptions = new List<Exception>();

            foreach (var subscriber in subscribers)
            {
                try
                {
                    await subscriber.Handler.Invoke(subscriber.Subscriber, message);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            return exceptions.ToImmutableList();
        }

        /// <inheritdoc />
        public void Unregister<TMessage>(object subscriber)
            where TMessage : class
        {
            Unregister<TMessage, DefaultChannel>(subscriber, DefaultChannel.Instance);
        }

        /// <inheritdoc />
        public void Unregister<TMessage, TChannel>(object subscriber, TChannel channel)
            where TMessage : class
            where TChannel : IEquatable<TChannel>
        {
            _registrationLock.Wait();
            try
            {
                var table = GetWeakReferenceTable<TMessage, TChannel>(channel);
                table.Remove(subscriber);
            }
            finally
            {
                _registrationLock.Release();
            }
        }

        /// <inheritdoc />
        public void UnregisterAll(object subscriber)
        {
            _registrationLock.Wait();
            try
            {
                foreach (var channelMap in _subscribers.Values)
                {
                    foreach (var table in channelMap.Values)
                    {
                        _ = table.Remove(subscriber);
                    }
                }
            }
            finally
            {
                _registrationLock.Release();
            }
        }

        /// <inheritdoc />
        public void UnregisterAll<TChannel>(object subscriber, TChannel channel)
            where TChannel : IEquatable<TChannel>
        {
            _registrationLock.Wait();
            try
            {
                var channels = _subscribers.Where(x => x.Key.ChannelType == typeof(TChannel));

                foreach (var channelMap in channels)
                {
                    if (channelMap.Value.TryGetValue(channel, out var table))
                    {
                        _ = table.Remove(subscriber);
                    }
                }
            }
            finally
            {
                _registrationLock.Release();
            }
        }

        private async Task<IEnumerable<SubscriberReference<TMessage>>> GetSubscribers<TMessage, TChannel>(TChannel channel)
            where TMessage : class
            where TChannel : IEquatable<TChannel>
        {
            await _registrationLock.WaitAsync();
            try
            {
                var table = GetWeakReferenceTable<TMessage, TChannel>(channel);
                return table.GetSubscribers<TMessage>();
            }
            finally
            {
                _registrationLock.Release();
            }
        }

        private WeakReferenceTable GetWeakReferenceTable<TMessage, TChannel>(TChannel channel)
            where TMessage : class
            where TChannel : IEquatable<TChannel>
        {
            var key = new CompositeKey(typeof(TMessage), typeof(TChannel));
            var channelMap = _subscribers.GetOrAdd(key, k => new());
            return channelMap.GetOrAdd(channel, key => new());
        }
    }
}
