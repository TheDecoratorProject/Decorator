using Decorator.ModuleAPI;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Decorator.Tests
{
	public class IgnoredLogic
	{
		[Fact]
		public void IsLogicless()
		{
			var t = new IgnoredAttribute.IgnoredLogic();

			t.OriginalType
				.Should()
				.BeNull();

			t.ModifiedType
				.Should()
				.BeNull();

			t.Member
				.Should()
				.Be(default(Member));
		}
	}
}
