using Decorator.Attributes;
using Decorator.Modules;

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
		private static object[] GetAndCorrupt(bool il, int pos)
			=> Helpers.GenerateAndCorrupt<ArrayClass>(il, pos);

		private static int GetEndsOn(bool il, object[] pos)
			=> Helpers.EndsOn<ArrayClass>(il, pos);

		[Fact]
		public void TypesAreCorrect()
		{
			var props = Helpers.GetProperties<ArrayClass>();
			var members = TestConverter<ArrayClass>.Members;

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

				members[i].GetType().GenericTypeArguments[0]
					.Should()
					.Be(ArrayClass.TypeSetup[i]
						.GetElementType());
			}
		}

		[Fact]
		public void EndsOnCorrectPosition()
		{
			for (var position = 0; position < Helpers.LengthOfDefault<ArrayClass>(false); position++)
			{
				GetEndsOn(false, GetAndCorrupt(false, position))
					.Should().Be(position);

				GetEndsOn(false, GetAndCorrupt(true, position))
					.Should().Be(position);

				GetEndsOn(true, GetAndCorrupt(false, position))
					.Should().Be(position);

				GetEndsOn(true, GetAndCorrupt(true, position))
					.Should().Be(position);
			}

			for (var position = 0; position < Helpers.LengthOfDefault<ArrayClass>(true); position++)
			{
				GetEndsOn(false, GetAndCorrupt(false, position))
					.Should().Be(position);

				GetEndsOn(false, GetAndCorrupt(true, position))
					.Should().Be(position);

				GetEndsOn(true, GetAndCorrupt(false, position))
					.Should().Be(position);

				GetEndsOn(true, GetAndCorrupt(true, position))
					.Should().Be(position);
			}
		}
	}
}