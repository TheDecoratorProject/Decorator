using Decorator.Attributes;

namespace Decorator.Demo.MessageTypes
{
    [Message("chat")]
    public class Chat
    {
        [Position(0)]
        public uint PlayerId { get; set; }

        [Position(1)]
        public string ChatMessage { get; set; }

        public override string ToString()
            => $"[{nameof(Chat)}]: {nameof(PlayerId)}: {PlayerId} {nameof(ChatMessage)}: {ChatMessage}";
    }
}