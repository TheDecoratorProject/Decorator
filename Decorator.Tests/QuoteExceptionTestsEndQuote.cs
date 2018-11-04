using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Decorator.Tests
{

	// gotta get that 100% code coverage amirite
	
	public class QuoteExceptionTestsEndQuote
	{
		public const string InvalidDeclarationDefault
			= "The class isn't defined properly";

		public const string NoDefaultConstructorDefault
			= "There was no default constructor specified.";

		public const string BrokenAttributePairingDefault
			= "There should be another attribute paired with the current attribute, however it is not there";

		public const string IrrationalAttributeDefault
			= "The attribute(s) order, value(s), or usage(s) is/are irrational.";

		public const string IrrationalAttributeValueDefault
			= "An invalid value has been specified for an attribute";

		[Fact]
		public void CanThrow_BrokenAttributePairingException()
			=> ((Action)(() =>
			{
				throw new BrokenAttributePairingException();
			})).Should().ThrowExactly<BrokenAttributePairingException>()
						.WithMessage(BrokenAttributePairingDefault);

		[Fact]
		public void CanThrow_IrrationalAttributeException()
			=> ((Action)(() =>
			{
				throw new IrrationalAttributeException();
			})).Should().ThrowExactly<IrrationalAttributeException>()
						.WithMessage(IrrationalAttributeDefault);

		[Fact]
		public void CanThrow_IrrationalAttributeValueException()
			=> ((Action)(() =>
			{
				throw new IrrationalAttributeValueException();
			})).Should().ThrowExactly<IrrationalAttributeValueException>()
						.WithMessage(IrrationalAttributeValueDefault);

		[Fact]
		public void CanThrow_InvalidDeclarationException()
			=> ((Action)(() =>
			{
				throw new InvalidDeclarationException();
			})).Should().ThrowExactly<InvalidDeclarationException>()
						.WithMessage(InvalidDeclarationDefault);

		[Fact]
		public void CanThrow_NoDefaultConstructorException()
			=> ((Action)(() =>
			{
				throw new NoDefaultConstructorException();
			})).Should().ThrowExactly<NoDefaultConstructorException>()
						.WithMessage(NoDefaultConstructorDefault);
	}
}
