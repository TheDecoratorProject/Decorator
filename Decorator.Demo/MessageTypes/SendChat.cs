using Decorator.Attributes;

namespace Decorator.Demo.MessageTypes
{
    [Message("chat")]
	public class SendChat
	{
		[Position(0)]
		public string ChatMessage { get; set; }

		[Position(1)]
		public uint ClientId { get; set; }
	}
}