using Gizmo.RemoteControl.Agent.Shared.Abstractions;

namespace Gizmo.RemoteControl.Agent.Shared.Services.Messenger.Private
{
    record SubscriberReference<TMessage>(object Subscriber, RegistrationCallback<TMessage> Handler);
}
