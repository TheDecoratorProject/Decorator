using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Decorator.Tests
{
	public class DeserializationTests
	{
		public class DeserializationTestsRequiredAttributeBase : IDecorable
		{
			[Position(0), Required]
			public string MyReferenceType { get; set; }

			[Position(1), Required]
			public int MyValueType { get; set; }
		}

		public class DeserializationTestsOptionalAttributeBase : IDecorable
		{
			[Position(0), Optional]
			public string MyReferenceType { get; set; }

			[Position(1), Optional]
			public int MyValueType { get; set; }
		}

		public class DeserializationTestsArrayAttributeBase : IDecorable
		{
			[Position(0), Array]
			public string[] MyReferenceTypes { get; set; }

			[Position(1), Array]
			public int[] MyValueTypes { get; set; }
		}

		public class DeserializationTestsFlattenAttributeBase : IDecorable
		{
			[Position(0), Flatten]
			public DeserializationTestsRequiredAttributeBase RequiredDecorable { get; set; }

			[Position(1), Flatten]
			public DeserializationTestsOptionalAttributeBase OptionalDecorable { get; set; }
		}

		public class DeserializationTestsFlattenArrayAttributeBase : IDecorable
		{
			[Position(0), FlattenArray]
			public DeserializationTestsRequiredAttributeBase[] RequiredDecorable { get; set; }

			[Position(1), FlattenArray]
			public DeserializationTestsArrayAttributeBase[] ArrayDecorable { get; set; }
		}

		[Theory]
		[InlineData("Deserializes types", "", 0)]
		[InlineData("Can set reference types to null", null, 0)]
		public void Required(string comment, params object[] deserializeInfo)
			=> Converter<DeserializationTestsRequiredAttributeBase>.TryDeserialize(deserializeInfo, out _)
				.Should().BeTrue(comment);

		[Theory]
		[InlineData("Normal Deserialization", "", 0)]
		[InlineData("Literally anything else", null, null)]
		[InlineData("Literally anything else", "", null)]
		[InlineData("Literally anything else", "", "")]
		[InlineData("Literally anything else", null, "")]
		[InlineData("Literally anything else", 0, null)]
		[InlineData("Literally anything else", 0, 0)]
		[InlineData("Literally anything else", null, 0)]
		[InlineData("Literally anything else", 0, "")]
		public void Optional(string comment, params object[] deserializeInfo)
			=> Converter<DeserializationTestsOptionalAttributeBase>.TryDeserialize(deserializeInfo, out _)
				.Should().BeTrue(comment);

		[Theory]
		[InlineData("Normal", 5, "a", "b", "c", "d", "e", 3, 1, 2, 3)]
		[InlineData("Reference types can be null", 2, null, null, 3, 1, 2, 3)]
		public void Array(string comment, params object[] deserializeInfo)
			=> Converter<DeserializationTestsArrayAttributeBase>.TryDeserialize(deserializeInfo, out _)
				.Should().BeTrue(comment);

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
			=> Converter<DeserializationTestsFlattenAttributeBase>.TryDeserialize(deserializeInfo, out _)
				.Should().BeTrue(comment);

		[Theory]
		[InlineData("it works, ok?", 4, "a", 1, "b", 2, "c", 3, "d", 4, 5, null, null, "b", 2, "c", "c", 4, 4, 5u, 5f, "f", 5f)]
		public void FlattenArray(string comment, params object[] deserializeInfo)
			=> Converter<DeserializationTestsFlattenArrayAttributeBase>.TryDeserialize(deserializeInfo, out _)
				.Should().BeTrue(comment);
	}
}
