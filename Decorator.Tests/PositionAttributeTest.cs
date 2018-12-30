using FluentAssertions;

using Xunit;

namespace Decorator.Tests.StretchToThat100PercentCodeCoverage
{
	public class PositionAttributeTest
	{
		public const int PositionNumber = 517390;

		[Fact]
		public void Works()
			=> new PositionAttribute(PositionNumber).Position
			.Should()
			.Be(PositionNumber);
	}
}