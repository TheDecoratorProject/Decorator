using Decorator.Attributes;

namespace Decorator.Tests
{
    public class NeedsAttribute
	{
		[Position(0), Required]
		public string WOWOW { get; set; }
	}
}