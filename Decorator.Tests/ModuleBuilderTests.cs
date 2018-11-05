using Decorator.ModuleAPI;

using FluentAssertions;

using System;
using System.Reflection;

using Xunit;

namespace Decorator.Tests
{
	public class ModuleBuilderTests
	{
		public string TestMember { get; set; }

		public PropertyInfo GetTestMember()
			=> typeof(ModuleBuilderTests)
				.GetProperty(nameof(TestMember));

		[Fact]
		public void Throws_InvalidDeclarationException_When_MemberInfoIsNull()
			=> ((Action)(() =>
			{
				ModuleBuilder.Build(null, null);
			})).Should().ThrowExactly<InvalidDeclarationException>();

		[Fact]
		public void Throws_ArgumentNullException_When_ModuleBuilderIsNull()
			=> ((Action)(() =>
			{
				ModuleBuilder.Build(GetTestMember(), null);
			})).Should().ThrowExactly<ArgumentNullException>();
	}
}