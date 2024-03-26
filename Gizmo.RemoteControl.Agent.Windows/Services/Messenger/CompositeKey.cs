namespace Gizmo.RemoteControl.Agent.Windows.Services.Messenger
{
    /// <summary>
    /// A dictionary key composed of the message type and channel type.
    /// </summary>
    readonly struct CompositeKey(Type messageType, Type channelType) : IEquatable<CompositeKey>
    {
        public Type MessageType { get; } = messageType;
        public Type ChannelType { get; } = channelType;

        public bool Equals(CompositeKey other)
        {
            return
                MessageType == other.MessageType &&
                ChannelType == other.ChannelType;
        }

        public override bool Equals(object? obj)
        {
            return
                obj is CompositeKey other &&
                Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MessageType, ChannelType);
        }

        public static bool operator ==(CompositeKey left, CompositeKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CompositeKey left, CompositeKey right)
        {
            return !left.Equals(right);
        }
    }
}
