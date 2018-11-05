using FluentAssertions;

using System;

using Xunit;

namespace Decorator.Tests.ModuleTests
{
	public class FlattenClass : IDecorable
	{
		public class InnerFlattenTypeOne : IDecorable
		{
			[Position(0), Required]
			public string ReferenceType { get; set; }

			[Position(1), Required]
			public int ValueType { get; set; }
		}

		public class InnerFlattenTypeTwo : IDecorable
		{
			[Position(0), Required]
			public int ValueType { get; set; }

			[Position(1), Required]
			public string ReferenceType { get; set; }
		}

		public static Type[] TypeSetup = new[]
		{
			typeof(InnerFlattenTypeOne),
			typeof(InnerFlattenTypeTwo),
		};

		[Position(0), Flatten]
		public InnerFlattenTypeOne InnerFlattenTypeOneItem { get; set; }
			= new InnerFlattenTypeOne
			{
				ReferenceType = "aaaa",
				ValueType = 1337,
			};

		[Position(1), Flatten]
		public InnerFlattenTypeTwo InnerFlattenTypeTwoItem { get; set; }
			= new InnerFlattenTypeTwo
			{
				ValueType = 7331,
				ReferenceType = "bbbb",
			};
	}

	public class FlattenTests
	{
		private object[] GetAndCorrupt(int pos)
			=> Helpers.GenerateAndCorrupt<FlattenClass>(pos);

		private int GetEndsOn(object[] pos)
			=> Helpers.EndsOn<FlattenClass>(pos);

		[Fact]
		public void TypesAreCorrect()
		{
			var members = DecoratorModuleContainer<FlattenClass>.Members;

			for (var i = 0; i < members.Count; i++)
			{
				members[i].OriginalType
					.Should()
					.Be(FlattenClass.TypeSetup[i]);

				members[i].ModifiedType
					.Should()
					.Be(FlattenClass.TypeSetup[i]);
			}
		}

		[Fact]
		public void EndsOnCorrectPosition()
		{
			for (var position = 0; position < Helpers.LengthOfDefault<FlattenClass>(); position++)
			{
				GetEndsOn(GetAndCorrupt(position))
					.Should().Be(position);
			}
		}
	}
}