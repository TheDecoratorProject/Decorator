using Decorator.Attributes;
using Decorator.Modules;

using FluentAssertions;

using System;

using Xunit;

namespace Decorator.Tests
{
	public class DeserializationTests
	{
		public class DeserializationTestsIgnoredAttributeBase
		{
			[Position(0), Ignored]
			public string MyReferenceType { get; set; }

			[Position(1), Ignored]
			public int MyValueType { get; set; }
		}

		public class DeserializationTestsRequiredAttributeBase
		{
			[Position(0), Required]
			public string MyReferenceType { get; set; }

			[Position(1), Required]
			public int MyValueType { get; set; }
		}

		public class DeserializationTestsOptionalAttributeBase
		{
			[Position(0), Optional]
			public string MyReferenceType { get; set; }

			[Position(1), Optional]
			public int MyValueType { get; set; }
		}

		public class DeserializationTestsArrayAttributeBase
		{
			[Position(0), Array]
			public string[] MyReferenceTypes { get; set; }

			[Position(1), Array]
			public int[] MyValueTypes { get; set; }
		}

		public class DeserializationTestsFlattenAttributeBase
		{
			[Position(0), Flatten]
			public DeserializationTestsRequiredAttributeBase RequiredDecorable { get; set; }

			[Position(1), Flatten]
			public DeserializationTestsOptionalAttributeBase OptionalDecorable { get; set; }
		}

		public class DeserializationTestsFlattenArrayAttributeBase
		{
			[Position(0), FlattenArray]
			public DeserializationTestsRequiredAttributeBase[] RequiredDecorable { get; set; }

			[Position(1), FlattenArray]
			public DeserializationTestsOptionalAttributeBase[] ArrayDecorable { get; set; }
		}

		[Theory]
		[InlineData("Nothing", new object[] { null, null })]
		[InlineData("Stuff", new object[] { "abcd", 1234, null })]
		public void Ignored(string comment, params object[] deserializeInfo)
		{
			TestConverter<DeserializationTestsIgnoredAttributeBase>.TryDeserialize(false, deserializeInfo, out _)
				  .Should().BeTrue(comment);

			TestConverter<DeserializationTestsIgnoredAttributeBase>.TryDeserialize(true, deserializeInfo, out _)
				  .Should().BeTrue(comment);
		}

		[Theory]
		[InlineData("Deserializes types", "", 0)]
		[InlineData("Can set reference types to null", null, 0)]
		public void Required(string comment, params object[] deserializeInfo)
		{
			TestConverter<DeserializationTestsRequiredAttributeBase>.TryDeserialize(false, deserializeInfo, out _)
				  .Should().BeTrue(comment);

			TestConverter<DeserializationTestsRequiredAttributeBase>.TryDeserialize(true, deserializeInfo, out _)
				  .Should().BeTrue(comment);
		}

		[Theory]
		[InlineData("Normal Deserialization", "a", 1)]
		[InlineData("1, Literally anything else", null, null)]
		[InlineData("2, Literally anything else", "a", null)]
		[InlineData("3, Literally anything else", "a", "a")]
		[InlineData("4, Literally anything else", null, "a")]
		[InlineData("5, Literally anything else", 1, null)]
		[InlineData("6, Literally anything else", 1, 1)]
		[InlineData("7, Literally anything else", null, 1)]
		[InlineData("8, Literally anything else", 1, "a")]
		[InlineData("Not the right reference type", 5f, 1)]
		[InlineData("Not the right value type", "a", 5f)]
		[InlineData("Not the right types", 5f, 5f)]
		public void Optional(string comment, params object[] deserializeInfo)
		{
			((Action)(() =>
			{
				TestConverter<DeserializationTestsOptionalAttributeBase>.TryDeserialize(false, deserializeInfo, out var item)
				  .Should().BeTrue($"NO IL - {comment}");

				if (deserializeInfo[0] is string str)
				{
					item.MyReferenceType
						.Should().Be(str);
				}

				if (deserializeInfo[1] is int intg)
				{
					item.MyValueType
						.Should().Be(intg);
				}
			})).Should().NotThrow("NO IL");

			((Action)(() =>
			{
				TestConverter<DeserializationTestsOptionalAttributeBase>.TryDeserialize(true, deserializeInfo, out var item)
				  .Should().BeTrue($"IL - {comment}");

				if (deserializeInfo[0] is string str)
				{
					item.MyReferenceType
						.Should().Be(str);
				}

				if (deserializeInfo[1] is int intg)
				{
					item.MyValueType
						.Should().Be(intg);
				}

			})).Should().NotThrow("IL");
		}

		[Theory]
		[InlineData("Normal", 5, "a", "b", "c", "d", "e", 3, 1, 2, 3)]
		[InlineData("Reference types can be null", 2, null, null, 3, 1, 2, 3)]
		[InlineData("No array items", 0, 0)]
		public void Array(string comment, params object[] deserializeInfo)
		{
			TestConverter<DeserializationTestsArrayAttributeBase>.TryDeserialize(false, deserializeInfo, out _)
				  .Should().BeTrue(comment);

			TestConverter<DeserializationTestsArrayAttributeBase>.TryDeserialize(true, deserializeInfo, out _)
				  .Should().BeTrue(comment);
		}

		[Theory]
		[InlineData("Normal Deserialization", "", 0, "", 0)]
		[InlineData("Literally anything else", "", 0, null, null)]
		[InlineData("Literally anything else", "", 0, "", null)]
		[InlineData("Literally anything else", "", 0, "", "")]
		[InlineData("Literally anything else", "", 0, null, "")]
		[InlineData("Literally anything else", "", 0, 0, null)]
		[InlineData("Literally anything else", "", 0, 0, 0)]
		[InlineData("Literally anything else", "", 0, null, 0)]
		[InlineData("Literally anything else", "", 0, 0, "")]
		[InlineData("With null", null, 0, "", 0)]
		public void Flatten(string comment, params object[] deserializeInfo)
		{
			TestConverter<DeserializationTestsFlattenAttributeBase>.TryDeserialize(false, deserializeInfo, out _)
				  .Should().BeTrue(comment);

			TestConverter<DeserializationTestsFlattenAttributeBase>.TryDeserialize(true, deserializeInfo, out _)
				  .Should().BeTrue(comment);
		}

		[Theory]
		[InlineData("No array items", 0, 0)]
		[InlineData("Single Items", 1, "a", 1, 1, "b", 2)]
		[InlineData("Null for reference type", 1, null, 1, 1, null, 2)]
		[InlineData("Optional works", 1, "a", 1, 2, "", 1, null, null)]
		[InlineData("Complex, should work", 4, "a", 1, "b", 2, "c", 3, "d", 4, 5, "c", 3, "b", 2, "c", "c", 4, 4, 5u, 5f, "f", 5f)]
		public void FlattenArray(string comment, params object[] deserializeInfo)
		{
			TestConverter<DeserializationTestsFlattenArrayAttributeBase>.TryDeserialize(false, deserializeInfo, out _)
				  .Should().BeTrue(comment);

			TestConverter<DeserializationTestsFlattenArrayAttributeBase>.TryDeserialize(true, deserializeInfo, out _)
				  .Should().BeTrue(comment);
		}

		public class HasNoMembers
		{
		}

		[Theory]
		[InlineData("Nothing in object array", new object[] { })]
		[InlineData("Stuff in object array", new object[] { 1, "2", 3, "4" })]
		[InlineData("Everything should just work", new object[] { null })]
		public void NoMembers(string comment, params object[] deserializeInfo)
		{
			TestConverter<HasNoMembers>.TryDeserialize(false, deserializeInfo, out _)
				  .Should().BeTrue(comment);

			TestConverter<HasNoMembers>.TryDeserialize(true, deserializeInfo, out _)
				  .Should().BeTrue(comment);
		}
	}
}