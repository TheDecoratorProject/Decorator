using Decorator.Attributes;

namespace Decorator.Tests
{
    [Message("non-existant")]
	public class NonExistant
	{
		[Position(0), Required]
		public string AAA { get; set; }
	}
}