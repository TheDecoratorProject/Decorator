﻿using FluentAssertions;

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
		private static object[] GetAndCorrupt(int pos)
			=> Helpers.GenerateAndCorrupt<FlattenClass>(pos);

		private static int GetEndsOn(object[] pos)
			=> Helpers.EndsOn<FlattenClass>(pos);

		[Fact]
		public void TypesAreCorrect()
		{
			var props = Helpers.GetProperties<FlattenClass>();
			var members = DConverter<FlattenClass>.Members;

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
			for (var position = 0; position < Helpers.LengthOfDefault<FlattenClass>(); position++)
			{
				GetEndsOn(GetAndCorrupt(position))
					.Should().Be(position);
			}
		}
	}
}