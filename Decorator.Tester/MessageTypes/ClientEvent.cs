using Decorator.Attributes;

namespace Decorator.Tester {

	[Message("ce")]
	public class ClientEvent {

		[Required]
		[Position(0)]
		public uint Id { get; set; }

		[Required]
		[Position(1)]
		public string Username { get; set; }

		[Required]
		[Position(2)]
		public bool JoinState { get; set; }
	}
}