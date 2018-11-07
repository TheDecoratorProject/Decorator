using Decorator.ModuleAPI;

using FluentAssertions;

using Xunit;

namespace Decorator.Tests
{
	public class IgnoredLogic
	{
		[Fact]
		public void IsLogicless()
		{
			var t = new IgnoredAttribute.IgnoredLogic();

			t.ModuleContainer
				.Should()
				.BeNull();
		}
	}
}