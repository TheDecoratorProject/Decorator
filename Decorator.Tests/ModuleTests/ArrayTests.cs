using FluentAssertions;

using System;

using Xunit;

namespace Decorator.Tests.ModuleTests
{
	public class ArrayClass
	{
		public static Type[] TypeSetup = new[]
		{
			typeof(string[]),
			typeof(int[]),
		};

		[Position(0), Array]
		public string[] ReferenceTypes { get; set; }
			= new string[]
			{
				"a", "b", "c", "d"
			};

		[Position(1), Array]
		public int[] ValueTypes { get; set; }
			= new int[]
			{
				1337, 1338, 1339, 13310
			};
	}

	public class ArrayTests
	{
		private static object[] GetAndCorrupt(int pos)
			=> Helpers.GenerateAndCorrupt<ArrayClass>(pos);

		private static int GetEndsOn(object[] pos)
			=> Helpers.EndsOn<ArrayClass>(pos);

		[Fact]
		public void TypesAreCorrect()
		{
			var props = Helpers.GetProperties<ArrayClass>();
			var members = DConverter<ArrayClass>.Members;

			for (var i = 0; i < ArrayClass.TypeSetup.Length; i++)
			{
				members[i].ModuleContainer.Member.GetMember
					.Should().Be(props[i]);
			}

			for (var i = 0; i < members.Count; i++)
			{
				members[i].ModuleContainer.Member.MemberType
					.Should()
					.Be(ArrayClass.TypeSetup[i]);

				members[i].ModuleContainer.ModifiedType
					.Should()
					.Be(ArrayClass.TypeSetup[i]
						.GetElementType());
			}
		}

		[Fact]
		public void EndsOnCorrectPosition()
		{
			for (var position = 0; position < Helpers.LengthOfDefault<ArrayClass>(); position++)
			{
				GetEndsOn(GetAndCorrupt(position))
					.Should().Be(position);
			}
		}
	}
}