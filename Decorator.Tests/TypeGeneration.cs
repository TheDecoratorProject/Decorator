using Decorator.ModuleAPI;
using FluentAssertions;

using System;

using Xunit;

namespace Decorator.Tests
{
	public class TypeGeneration
	{
		public class FlattensNonDecorable : IDecorable
		{
			public FlattensNonDecorable() => NonDecorableValue = new NonDecorable();

			[Position(0), Flatten]
			public NonDecorable NonDecorableValue { get; set; }

			public class NonDecorable
			{
				[Position(0), Required]
				public int NoDecorableAttribute;
			}
		}

		[Fact]
		public void Throws_InvalidDeclarationException_When_AttemptingToFlattenAClassWithoutDecorableInterface()
		{
			((Action)(() =>
			{
				new ConverterContainer()
					.Request<FlattensNonDecorable>();
			})).Should().ThrowExactly<InvalidDeclarationException>();
		}

		public class NoSpecificationOnPositionAttribute : IDecorable
		{
			[Position(0)]
			public string ReferenceType { get; set; }
		}

		[Fact]
		public void Throws_BrokenAttributePairingException_When_PositionAttributeLacksAnAttributeModifier()
		{
			((Action)(() =>
			{
				new ConverterContainer().Request<NoSpecificationOnPositionAttribute>();
			})).Should().ThrowExactly<BrokenAttributePairingException>();
		}

		public class DuplicatePositionAttributes : IDecorable
		{
			[Position(0), Required]
			public string ReferenceType { get; set; }

			[Position(0), Required]
			public int ValueType { get; set; }
		}

		[Fact]
		public void Throws_IrrationalAttributeValueException_When_DuplicatePositionsWithSameValue()
		{
			((Action)(() =>
			{
				new ConverterContainer().Request<DuplicatePositionAttributes>();
			})).Should().ThrowExactly<IrrationalAttributeValueException>();
		}

		public class IrrationalPositionAttribute : IDecorable
		{
			public IrrationalPositionAttribute()
			{
			}

			[Position(int.MinValue), Required]
			public string ReferenceType { get; set; }
		}

		[Fact]
		public void Throws_IrrationalAttributeValueException_When_PositionIsLessThanOne()
		{
			((Action)(() =>
			{
				new ConverterContainer().Request<IrrationalPositionAttribute>();
			})).Should().ThrowExactly<IrrationalAttributeValueException>();
		}

		public class TooManyPairings : IDecorable
		{
			public TooManyPairings()
			{
			}

			[Position(0), Required, Optional, Flatten, Array, FlattenArray]
			public string ReferenceType { get; set; }
		}

		[Fact]
		public void Throws_IrrationalAttributeException_When_ThereAreMoreThenOneModifiersOnAMember()
		{
			((Action)(() =>
			{
				new ConverterContainer().Request<TooManyPairings>();
			})).Should().ThrowExactly<IrrationalAttributeException>();
		}

		public class ArrayAppliedIncorrectly : IDecorable
		{
			[Position(0), Array]
			public string RefereceType { get; set; }
		}

		[Fact]
		public void Throws_InvalidDeclarationException_When_ArrayAppliedIncorrectly()
		{
			((Action)(() =>
			{
				new ConverterContainer().Request<ArrayAppliedIncorrectly>();
			})).Should().ThrowExactly<InvalidDeclarationException>();
		}

		public class FlattenArrayAppliedIncorrectly : IDecorable
		{
			[Position(0), FlattenArray]
			public string RefereceType { get; set; }
		}

		[Fact]
		public void Throws_InvalidDeclarationException_When_FlattenArrayAppliedIncorrectly()
		{
			((Action)(() =>
			{
				new ConverterContainer().Request<FlattenArrayAppliedIncorrectly>();
			})).Should().ThrowExactly<InvalidDeclarationException>();
		}

		public class PerfectlyFineClass : IDecorable
		{
			[Position(0), Required]
			public string ReferenceType { get; set; }

			[Position(1), Array]
			public int[] MyArray;

			[Position(3), Optional]
			public int Optional;
		}

		[Fact]
		public void Throws_Nothing_When_DefinedNormally()
		{
			((Action)(() =>
			{
				new ConverterContainer().Request<PerfectlyFineClass>();
			})).Should().NotThrow();
		}

		public class HasNoMembers : IDecorable
		{
		}

		public class AlsoHasNoMembers : HasNoMembers
		{
		}

		[Fact]
		public void Throws_Nothing_When_DefinedWithNothing()
		{
			((Action)(() =>
			{
				new ConverterContainer().Request<AlsoHasNoMembers>();
			})).Should().NotThrow();
		}
	}
}