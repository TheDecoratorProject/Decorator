using Decorator.Attributes;

namespace Decorator.Tester
{
	public class ClientEvent : ClientExistsEvent
	{
		[Required]
		[Position(2)]
		public bool JoinState { get; set; }
	}

	[Repeatable]
	[Message("ce")]
	public class ClientExistsEvent
	{
		[Required]
		[Position(0)]
		public uint Id { get; set; }

		[Optional]
		[Position(1)]
		public string Username { get; set; }
	}
}