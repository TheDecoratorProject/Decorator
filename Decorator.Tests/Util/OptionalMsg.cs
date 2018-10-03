using Decorator.Attributes;

namespace Decorator.Tests
{
    [Message("opt")]
	public class OptionalMsg
	{
		[Position(0), Required]
		public string RequiredString { get; set; }

		[Position(1), Optional]
		public int OptionalValue { get; set; }
	}
}