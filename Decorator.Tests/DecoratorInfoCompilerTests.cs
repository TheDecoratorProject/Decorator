using Decorator.Attributes;
using Decorator.Modules;

using FluentAssertions;

using Xunit;

namespace Decorator.Tests
{
	public class DecoratorModuleCompilerTests
	{
		public class NeedsFillingIn
		{
			[Position(0), Required]
			public int Property1 { get; set; }

			[Position(10), Required]
			public int Property10 { get; set; }
		}

		[Fact]
		public void FillsInWithIgnored()
		{
			var data = new object[] { 1, null, null, null, null, null, null, null, null, null, 10 };

			DConverter<NeedsFillingIn>.TryDeserialize(data, out var result)
				.Should().Be(true);

			result
				.Property1
				.Should().Be((int)data[0]);

			result
				.Property10
				.Should().Be((int)data[10]);
		}
	}
}