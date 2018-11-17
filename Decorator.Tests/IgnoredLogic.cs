using Decorator.ModuleAPI;

using FluentAssertions;

using Xunit;

namespace Decorator.Tests
{
	public class IgnoredLogicTests
	{
		[Fact]
		public void IsLogicless()
		{
			var t = new IgnoredLogic();

			t.ModuleContainer
				.Should()
				.BeNull();
		}
	}
}