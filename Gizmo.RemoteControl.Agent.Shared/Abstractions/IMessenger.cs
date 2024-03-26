using System.Collections.Immutable;

namespace Gizmo.RemoteControl.Agent.Shared.Abstractions
{
    public delegate Task RegistrationCallback<TMessage>(object subscriber, TMessage message);

    /// <summary>
    /// A service for sending and receiving messages between decoupled objects.
    /// automatically remove handlers when the subscriber is garbage-collected.
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// Whether the specified subscriber is registered for a particular
        /// message type and channel.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TChannel"></typeparam>
        /// <param name="subscriber"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        bool IsRegistered<TMessage, TChannel>(object subscriber, TChannel channel)
            where TMessage : class
            where TChannel : IEquatable<TChannel>;

        /// <summary>
        /// Whether the specified subscriber is registered for a particular
        /// message type under the default channel.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        bool IsRegistered<TMessage>(object subscriber)
            where TMessage : class;

        /// <summary>
        /// Registers the subscriber using the specified message type, channel, and handler.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TChannel"></typeparam>
        /// <param name="subscriber"></param>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <returns>A token that, when disposed, will unregister the handler.</returns>
        IDisposable Register<TMessage, TChannel>(
            object subscriber,
            TChannel channel,
            RegistrationCallback<TMessage> handler)
                where TMessage : class
                where TChannel : IEquatable<TChannel>;

        /// <summary>
        /// Registers the subscriber using the specified message type and handler,
        /// under the default channel.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="subscriber"></param>
        /// <param name="handler"></param>
        /// <returns>A token that, when disposed, will unregister the handler.</returns>
        IDisposable Register<TMessage>(object subscriber, RegistrationCallback<TMessage> handler)
            where TMessage : class;

        /// <summary>
        /// Sends a message to the specified channel.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TChannel"></typeparam>
        /// <param name="message"></param>
        /// <param name="channel"></param>
        /// <returns>A list of exceptions, if any, that occurred while invoking the handlers.</returns>
        Task<IImmutableList<Exception>> Send<TMessage, TChannel>(TMessage message, TChannel channel)
            where TMessage : class
            where TChannel : IEquatable<TChannel>;

        /// <summary>
        /// Sends a message to the default channel.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <returns>A list of exceptions, if any, that occurred while invoking the handlers.</returns>
        Task<IImmutableList<Exception>> Send<TMessage>(TMessage message)
            where TMessage : class;

        /// <summary>
        /// Unregistered the subscriber from the specified message type and channel.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TChannel"></typeparam>
        /// <param name="subscriber"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        void Unregister<TMessage, TChannel>(object subscriber, TChannel channel)
            where TMessage : class
            where TChannel : IEquatable<TChannel>;

        /// <summary>
        /// Unregistered the subscriber from the default channel.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        void Unregister<TMessage>(object subscriber)
            where TMessage : class;

        /// <summary>
        /// Unregister all handlers for the subscriber on the default channel.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        void UnregisterAll(object subscriber);

        /// <summary>
        /// Unregister all handlers for the subscriber on the given channel.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        void UnregisterAll<TChannel>(object subscriber, TChannel channel)
            where TChannel : IEquatable<TChannel>;
    }
}
