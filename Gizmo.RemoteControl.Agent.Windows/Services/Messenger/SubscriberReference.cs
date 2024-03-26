using Gizmo.RemoteControl.Agent.Shared.Abstractions;

namespace Gizmo.RemoteControl.Agent.Windows.Services.Messenger
{
    record SubscriberReference<TMessage>(object Subscriber, RegistrationCallback<TMessage> Handler);
}
