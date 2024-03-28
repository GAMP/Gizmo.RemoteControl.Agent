using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Gizmo.RemoteControl.Agent.Shared.Abstractions;

namespace Gizmo.RemoteControl.Agent.Shared.Services.Messenger.Private
{
    class WeakReferenceTable
    {
        private readonly object _tableLock = new();
        private readonly ConditionalWeakTable<object, object?> _weakTable = [];

        internal void AddOrUpdate<TMessage>(object subscriber, RegistrationCallback<TMessage> callback)
        {
            lock (_tableLock)
            {
                _weakTable.AddOrUpdate(subscriber, callback);
            }
        }

        internal IEnumerable<SubscriberReference<TMessage>> GetSubscribers<TMessage>()
        {
            lock (_tableLock)
            {
                return _weakTable
                    .Where(x => x.Value is RegistrationCallback<TMessage>)
                    .Select(x => new SubscriberReference<TMessage>(x.Key, (RegistrationCallback<TMessage>)x.Value!))
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
}
