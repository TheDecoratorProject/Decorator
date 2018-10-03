using Decorator.Attributes;

namespace Decorator.Tests
{
    [Message("rep")]
	public class NonRepeatable
	{
		[Position(0)]
		public int SomeValue { get; set; }
	}
}