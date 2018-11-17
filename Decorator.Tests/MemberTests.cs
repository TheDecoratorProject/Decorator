using Decorator.ModuleAPI;

using FluentAssertions;

using System;
using System.Reflection;

using Xunit;

namespace Decorator.Tests
{
	public class MemberTests
	{
		// i'm testing this stuff to make sure it works, but i really don't appreciate if you do what i'm doing :(

		[Fact]
		public void Throws_ArgumentNullException_When_ConstructingNullProperty()
		{
			((Action)(() =>
			{
				new Member((PropertyInfo)null);
			})).Should().ThrowExactly<ArgumentNullException>();
		}

		[Fact]
		public void Throws_ArgumentNullException_When_ConstructingNullField()
		{
			((Action)(() =>
			{
				new Member((FieldInfo)null);
			})).Should().ThrowExactly<ArgumentNullException>();
		}
	}
}