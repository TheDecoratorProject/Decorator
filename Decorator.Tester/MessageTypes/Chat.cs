using Decorator.Attributes;

namespace Decorator.Tester.MessageTypes {

	[Message("chat")]
	public class Chat {

		[Position(0)]
		public uint PlayerId { get; set; }

		[Position(1)]
		public string ChatMessage { get; set; }

		public override string ToString()
			=> $"[{nameof(Chat)}]: {nameof(this.PlayerId)}: {this.PlayerId} {nameof(this.ChatMessage)}: {this.ChatMessage}";
	}

	[Message("chat")]
	public class SendChat {

		[Position(0)]
		public string ChatMessage { get; set; }
		
		[Position(1)]
		public uint ClientId { get; set; }
	}
}