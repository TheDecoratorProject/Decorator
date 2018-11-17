using Decorator.ModuleAPI;

using FluentAssertions;

using System;

using Xunit;

namespace Decorator.Tests
{
	public class ModuleBuilderTests
	{
		public string TestMember { get; set; }

		public static BaseContainer GetTestMember()
			=> new Container(typeof(string), new Member(typeof(ModuleBuilderTests).GetProperty(nameof(TestMember))));

		[Fact]
		public void Throws_InvalidDeclarationException_When_MemberInfoIsNull()
			=> ((Action)(() =>
			{
				ModuleBuilder.Build(null, null);
			})).Should().ThrowExactly<ArgumentNullException>();

		[Fact]
		public void Throws_ArgumentNullException_When_ConverterContainerIsNull()
			=> ((Action)(() =>
			{
				ModuleBuilder.Build(GetTestMember(), null);
			})).Should().ThrowExactly<ArgumentNullException>();

		[Fact]
		public void Throws_ArgumentNullException_When_ModuleBuilderIsNull()
			=> ((Action)(() =>
			{
				ModuleBuilder.Build(GetTestMember(), null);
			})).Should().ThrowExactly<ArgumentNullException>();
	}
}