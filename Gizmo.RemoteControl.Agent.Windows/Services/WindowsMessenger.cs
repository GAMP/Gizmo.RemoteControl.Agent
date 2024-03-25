using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Gizmo.RemoteControl.Agent.Shared.Abstractions;
using Gizmo.RemoteControl.Agent.Shared.Messages;

namespace Gizmo.RemoteControl.Agent.Windows.Services
{
    readonly struct DefaultChannel : IEquatable<DefaultChannel>
    {
        private static readonly int _hashCode =
            HashCode.Combine(
                Guid.Parse("a40d3b94-6559-4d7f-9524-3c345467ab1c"),
                "DefaultChannel");

        public static DefaultChannel Instance { get; } = new();

        public bool Equals(DefaultChannel other)
        {
            return true;
        }

        public override bool Equals(object? obj)
        {
            return
                obj is DefaultChannel defaultChannel &&
                Equals(defaultChannel);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }

    class SubscriberReference<TMessage>
    {
        public SubscriberReference(object subscriber, Func<TMessage, Task> handler)
        {
            Subscriber = subscriber;
            Handler = handler;
        }

        public object Subscriber { get; }
        public Func<TMessage, Task> Handler { get; }
    }

    class WeakReferenceTable
    {
        private readonly object _tableLock = new();
        private readonly ConditionalWeakTable<object, object?> _weakTable = new();

        internal void AddOrUpdate<TMessage>(object subscriber, Func<TMessage, Task> handler)
        {
            lock (_tableLock)
            {
                _weakTable.AddOrUpdate(subscriber, handler);
            }
        }

        internal IEnumerable<Func<TMessage, Task>> GetSubscribers<TMessage>()
        {
            lock (_tableLock)
            {

                return _weakTable
                    .Where(x => x.Value is Func<TMessage, Task>)
                    .Select(x => new SubscriberReference<TMessage>(
                        x.Key,
                        (Func<TMessage, Task>)x.Value!))
                    .ToImmutableArray();
            }
        }


        internal bool Remove(object subscriber)
        {
            lock (_tableLock)
            {
                return _weakTable.Remove(subscriber);
            }
        }

        internal bool TryGetValue(object subscriber, [NotNullWhen(true)] out object? handler)
        {
            lock (_tableLock)
            {
                return _weakTable.TryGetValue(subscriber, out handler);
            }
        }
    }

    public sealed class WindowsMessenger : IMessenger
    {
        private readonly SemaphoreSlim _registrationLock = new(1, 1);

        public async Task<IAsyncDisposable> Register<T>(object _, Func<T, Task> handle) where T : class
        {
            await _registrationLock.WaitAsync();
            try
            {
                var table = GetWeakReferenceTable<TMessage, TChannel>(channel);

                if (table.TryGetValue(subscriber, out _))
                {
                    throw new InvalidOperationException(
                        "Subscriber is already registered to the specified message and channel.");
                }

                table.AddOrUpdate(subscriber, handler);

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

        public void Send(AppStateHostChangedMessage message)
        {
            throw new NotImplementedException();
        }

        public void Send(WindowsSessionSwitchedMessage message)
        {
            throw new NotImplementedException();
        }

        public void Send(WindowsSessionEndingMessage message)
        {
            throw new NotImplementedException();
        }

        public void Send(DisplaySettingsChangedMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
