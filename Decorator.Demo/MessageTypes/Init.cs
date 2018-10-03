using Decorator.Attributes;

namespace Decorator.Demo
{
	[Message("init")]
	public class InitEvent
	{
		[Required]
		[Position(0)]
		public uint MyId { get; set; }
	}
}