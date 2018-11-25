using Decorator.Attributes;
using Decorator.Modules;

using FluentAssertions;

using System;

using Xunit;

namespace Decorator.Tests.ModuleTests
{
	public class FlattenClass
	{
		public class InnerFlattenTypeOne
		{
			[Position(0), Required]
			public string ReferenceType { get; set; }

			[Position(1), Required]
			public int ValueType { get; set; }
		}

		public class InnerFlattenTypeTwo
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
		private static object[] GetAndCorrupt(bool il, int pos)
			=> Helpers.GenerateAndCorrupt<FlattenClass>(il, pos);

		private static int GetEndsOn(bool il, object[] pos)
			=> Helpers.EndsOn<FlattenClass>(il, pos);

		[Fact]
		public void TypesAreCorrect()
		{
			var props = Helpers.GetProperties<FlattenClass>();
			var members = TestConverter<FlattenClass>.Members;

			for (var i = 0; i < FlattenArrayClass.TypeSetup.Length; i++)
			{
				members[i].ModuleContainer.Member.GetMember
					.Should().Be(props[i]);
			}

			for (var i = 0; i < members.Count; i++)
			{
				members[i].ModuleContainer.Member.MemberType
					.Should()
					.Be(FlattenClass.TypeSetup[i]);

				members[i].GetType().GenericTypeArguments[0]
					.Should()
					.Be(FlattenClass.TypeSetup[i]);
			}
		}

		[Fact]
		public void EndsOnCorrectPosition()
		{
			for (var position = 0; position < Helpers.LengthOfDefault<FlattenClass>(false); position++)
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

			for (var position = 0; position < Helpers.LengthOfDefault<FlattenClass>(true); position++)
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