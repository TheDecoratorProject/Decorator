using FluentAssertions;

using System;

using Xunit;

namespace Decorator.Tests.ModuleTests
{
	public class RequiredClass
	{
		public static Type[] TypeSetup = new[]
		{
			typeof(string),
			typeof(int),
			typeof(string[]),
		};

		[Position(0), Required]
		public string ReferenceType { get; set; }
			= "hello";

		[Position(1), Required]
		public int ValueType { get; set; }
			= 1337;

		[Position(2), Required]
		public string[] Array { get; set; }
			= new string[] { "a", "b", "c" };
	}

	public class RequiredTests
	{
		private static object[] GetAndCorrupt(int pos)
			=> Helpers.GenerateAndCorrupt<RequiredClass>(pos);

		private static int GetEndsOn(object[] pos)
			=> Helpers.EndsOn<RequiredClass>(pos);

		[Fact]
		public void TypesAreCorrect()
		{
			var props = Helpers.GetProperties<RequiredClass>();
			var members = DConverter<RequiredClass>.Members;

			for (var i = 0; i < FlattenArrayClass.TypeSetup.Length; i++)
			{
				members[i].ModuleContainer.Member.GetMember
					.Should().Be(props[i]);
			}

			for (var i = 0; i < members.Count; i++)
			{
				members[i].ModuleContainer.Member.MemberType
					.Should().Be(RequiredClass.TypeSetup[i]);

				members[i].ModuleContainer.ModifiedType
					.Should().Be(RequiredClass.TypeSetup[i]);
			}
		}

		[Fact]
		public void EndsOnCorrectPosition()
		{
			for (var position = 0; position < Helpers.LengthOfDefault<RequiredClass>(); position++)
			{
				GetEndsOn(GetAndCorrupt(position))
					.Should().Be(position);
			}
		}
	}
}