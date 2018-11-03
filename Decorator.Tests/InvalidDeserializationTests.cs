using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Decorator.Tests
{
	public class InvalidDeserializationTests
	{
		public class InvalidDeserializationTestsRequiredAttributeBase : IDecorable
		{
			[Position(0), Required]
			public string MyReferenceType { get; set; }

			[Position(1), Required]
			public int MyValueType { get; set; }
		}

		public class InvalidDeserializationTestsOptionalAttributeBase : IDecorable
		{
			[Position(0), Optional]
			public string MyReferenceType { get; set; }

			[Position(1), Optional]
			public int MyValueType { get; set; }
		}

		public class InvalidDeserializationTestsArrayAttributeBase : IDecorable
		{
			[Position(0), Array]
			public string[] MyReferenceTypes { get; set; }

			[Position(1), Array]
			public int[] MyValueTypes { get; set; }
		}

		public class InvalidDeserializationTestsFlattenAttributeBase : IDecorable
		{
			[Position(0), Flatten]
			public InvalidDeserializationTestsRequiredAttributeBase RequiredDecorable { get; set; }

			[Position(1), Flatten]
			public InvalidDeserializationTestsOptionalAttributeBase OptionalDecorable { get; set; }
		}

		public class InvalidDeserializationTestsFlattenArrayAttributeBase : IDecorable
		{
			[Position(0), FlattenArray]
			public InvalidDeserializationTestsRequiredAttributeBase[] RequiredDecorable { get; set; }

			[Position(1), FlattenArray]
			public InvalidDeserializationTestsArrayAttributeBase[] ArrayDecorable { get; set; }
		}

		[Theory]
		[InlineData("Null parameter", null)]
		[InlineData("Not enough parameters", new object[] { })]
		[InlineData("Single null parameter", new object[] { null })]
		[InlineData("Single valid parameter", new object[] { "" })]
		[InlineData("Type Safety of Reference Type", "", "")]
		[InlineData("Type Safety of Value Type", 0, 0)]
		[InlineData("Can't set value types to null", "", null)]
		[InlineData("Not the right type for value type", "", 5f)]
		[InlineData("Not the right type for reference type", 5f, 0)]
		[InlineData("Not the right types", 5f, 5f)]
		public void Required(string comment, params object[] deserializeInfo)
			=> Converter<InvalidDeserializationTestsRequiredAttributeBase>.TryDeserialize(deserializeInfo, out _)
				.Should().BeFalse(comment);

		[Theory]
		[InlineData("Null parameter", null)]
		[InlineData("Not enough parameters", new object[] { })]
		[InlineData("Single null parameter", new object[] { null })]
		[InlineData("Single valid parameter", new object[] { "" })]
		[InlineData("Single invalid parameter", new object[] { 0 })]
		public void Optional(string comment, params object[] deserializeInfo)
			=> Converter<InvalidDeserializationTestsOptionalAttributeBase>.TryDeserialize(deserializeInfo, out _)
				.Should().BeFalse(comment);

		[Theory]
		[InlineData("Null parameter", null)]
		[InlineData("Not enough parameters", new object[] { })]
		[InlineData("Single null parameter", new object[] { null })]
		[InlineData("Invalid reference type array length", new object[] { 20, "" })]
		[InlineData("Invalid value type array length", new object[] { 1, "", 20, 0 })]
		[InlineData("No array length despite there being values", new object[] { 0, "", 0, 5 })]
		[InlineData("Null in value types", new object[] { 0, 1, null })]
		[InlineData("Not the right reference type", new object[] { 1, 5f, 0, 0 })]
		[InlineData("Not the right value type", new object[] { 1, "", 5f, 0 })]
		[InlineData("Not the right types", new object[] { 1, 5f, 5f, 0 })]
		[InlineData("Largest array size", new object[] { 3, "a", "b", "c", int.MaxValue })]
		[InlineData("Smallest array size", new object[] { 3, "a", "b", "c", int.MinValue })]
		public void Array(string comment, params object[] deserializeInfo)
			=> Converter<InvalidDeserializationTestsArrayAttributeBase>.TryDeserialize(deserializeInfo, out _)
				.Should().BeFalse(comment);

		[Theory]
		[InlineData("Null parameter", null)]
		[InlineData("Not enough parameters", new object[] { })]
		[InlineData("Single null parameter", new object[] { null })]
		[InlineData("Optionals aren't fufilled", new object[] { "", 0 })]
		public void Flatten(string comment, params object[] deserializeInfo)
			=> Converter<InvalidDeserializationTestsFlattenAttributeBase>.TryDeserialize(deserializeInfo, out _)
				.Should().BeFalse(comment);

		[Theory]
		[InlineData("Null parameter", null)]
		[InlineData("Not enough parameters", new object[] { })]
		[InlineData("Single null parameter", new object[] { null })]
		[InlineData("Invalid reference type array length", new object[] { 20, "", 0 })]
		[InlineData("Invalid value type array length", new object[] { 1, "", 0, 20, 0 })]
		[InlineData("No array length despite there being values", new object[] { 0, "", 0, 5, 0, 0 })]
		[InlineData("Null in value types", new object[] { 0, 1, null })]
		[InlineData("Illusion of there being objects", new object[] { 1, "", 0, 2 })]
		[InlineData("Not the right types", new object[] { 1, 5f, 5f, 1, 1, "", 0 })]
		[InlineData("Not the right reference type", new object[] { 1, "", 5f, 1, 1, "", 0 })]
		[InlineData("Not the right value type", new object[] { 1, 5f, 0, 1, 1, "", 0 })]
		[InlineData("Largest array size", new object[] { 3, "a", 1, "b", 2, "c", 3, int.MaxValue, int.MaxValue })]
		[InlineData("Smallest array size", new object[] { 3, "a", 1, "b", 2, "c", 3, int.MinValue, int.MinValue })]
		public void FlattenArray(string comment, params object[] deserializeInfo)
			=> Converter<InvalidDeserializationTestsFlattenArrayAttributeBase>.TryDeserialize(deserializeInfo, out _)
				.Should().BeFalse(comment);
	}
}
