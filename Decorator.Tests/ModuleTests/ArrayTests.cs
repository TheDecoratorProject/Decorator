using FluentAssertions;

using System;

using Xunit;

namespace Decorator.Tests.ModuleTests
{
	public class ArrayClass : IDecorable
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
		private object[] GetAndCorrupt(int pos)
			=> Helpers.GenerateAndCorrupt<ArrayClass>(pos);

		private int GetEndsOn(object[] pos)
			=> Helpers.EndsOn<ArrayClass>(pos);

		[Fact]
		public void TypesAreCorrect()
		{
			var members = DecoratorModuleContainer<ArrayClass>.Members;

			for (var i = 0; i < members.Count; i++)
			{
				members[i].OriginalType
					.Should()
					.Be(ArrayClass.TypeSetup[i]);

				members[i].ModifiedType
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