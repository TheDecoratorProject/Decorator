using Decorator.Attributes;

namespace Decorator.Demo
{
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