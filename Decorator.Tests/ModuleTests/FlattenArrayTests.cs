using FluentAssertions;

using System;

using Xunit;

namespace Decorator.Tests.ModuleTests
{
	public class FlattenArrayClass : IDecorable
	{
		public class InnerFlattenTypeOne : FlattenClass.InnerFlattenTypeOne
		{
			public InnerFlattenTypeOne()
			{
				ReferenceType = "aaaa";
				ValueType = 1337;
			}
		}

		public class InnerFlattenTypeTwo : FlattenClass.InnerFlattenTypeTwo
		{
			public InnerFlattenTypeTwo()
			{
				ValueType = 1337;
				ReferenceType = "aaaa";
			}
		}

		public static Type[] TypeSetup = new[]
		{
			typeof(InnerFlattenTypeOne[]),
			typeof(InnerFlattenTypeTwo[]),
		};

		[Position(0), FlattenArray]
		public InnerFlattenTypeOne[] InnerFlattenTypeOnes { get; set; }
			= new InnerFlattenTypeOne[]
			{
				new InnerFlattenTypeOne(),
				new InnerFlattenTypeOne(),
				new InnerFlattenTypeOne(),
				new InnerFlattenTypeOne(),
			};

		[Position(1), FlattenArray]
		public InnerFlattenTypeTwo[] InnerFlattenTypeTwos { get; set; }
			= new InnerFlattenTypeTwo[]
			{
				new InnerFlattenTypeTwo(),
				new InnerFlattenTypeTwo(),
				new InnerFlattenTypeTwo(),
				new InnerFlattenTypeTwo(),
			};
	}

	public class FlattenArrayTests
	{
		private object[] GetAndCorrupt(int pos)
			=> Helpers.GenerateAndCorrupt<FlattenArrayClass>(pos);

		private int GetEndsOn(object[] pos)
			=> Helpers.EndsOn<FlattenArrayClass>(pos);

		[Fact]
		public void TypesAreCorrect()
		{
			var props = Helpers.GetProperties<FlattenArrayClass>();
			var members = DConverter<FlattenArrayClass>.Members;

			for (var i = 0; i < FlattenArrayClass.TypeSetup.Length; i++)
			{
				members[i].Member.GetMember
					.Should().Be(props[i]);
			}

			for (var i = 0; i < members.Count; i++)
			{
				members[i].OriginalType
					.Should()
					.Be(FlattenArrayClass.TypeSetup[i]);

				members[i].ModifiedType
					.Should()
					.Be(FlattenArrayClass.TypeSetup[i]
						.GetElementType());
			}
		}

		[Fact]
		public void EndsOnCorrectPosition()
		{
			for (var position = 0; position < Helpers.LengthOfDefault<FlattenArrayClass>(); position++)
			{
				GetEndsOn(GetAndCorrupt(position))
					.Should().Be(position);
			}
		}
	}
}