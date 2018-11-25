﻿using Decorator.Attributes;
using Decorator.Modules;

using FluentAssertions;

using System;

using Xunit;

namespace Decorator.Tests.ModuleTests
{
	public class FlattenArrayClass
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
		private static object[] GetAndCorrupt(bool il, int pos)
			=> Helpers.GenerateAndCorrupt<FlattenArrayClass>(il, pos);

		private static int GetEndsOn(bool il, object[] pos)
			=> Helpers.EndsOn<FlattenArrayClass>(il, pos);

		[Fact]
		public void TypesAreCorrect()
		{
			var props = Helpers.GetProperties<FlattenArrayClass>();
			var members = TestConverter<FlattenArrayClass>.Members;

			for (var i = 0; i < FlattenArrayClass.TypeSetup.Length; i++)
			{
				members[i].ModuleContainer.Member.GetMember
					.Should().Be(props[i]);
			}

			for (var i = 0; i < members.Count; i++)
			{
				members[i].ModuleContainer.Member.MemberType
					.Should()
					.Be(FlattenArrayClass.TypeSetup[i]);

				members[i].GetType().GenericTypeArguments[0]
					.Should()
					.Be(FlattenArrayClass.TypeSetup[i]
						.GetElementType());
			}
		}

		[Fact]
		public void EndsOnCorrectPosition()
		{
			for (var position = 0; position < Helpers.LengthOfDefault<FlattenArrayClass>(false); position++)
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

			for (var position = 0; position < Helpers.LengthOfDefault<FlattenArrayClass>(true); position++)
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