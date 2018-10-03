using Decorator.Attributes;

namespace Decorator.Tests
{
	[Message("test"), Repeatable]
	public class TestMessage
	{
		[Position(0)]
		public string PositionZeroItem { get; set; }

		[Position(1)]
		public int PositionOneItem { get; set; }

		public float Ignored { get; set; }
	}
}