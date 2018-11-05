using FluentAssertions;

using System;

using Xunit;

namespace Decorator.Tests
{
	// gotta get that 100% code coverage amirite

	public class QuoteExceptionTestsEndQuote
	{
		[Fact]
		public void CanThrow_BrokenAttributePairingException()
			=> ((Action)(() =>
			{
				throw new BrokenAttributePairingException();
			})).Should().ThrowExactly<BrokenAttributePairingException>()
						.WithMessage("There should be another attribute paired with the current attribute, however it is not there");

		[Fact]
		public void CanThrow_IrrationalAttributeException()
			=> ((Action)(() =>
			{
				throw new IrrationalAttributeException();
			})).Should().ThrowExactly<IrrationalAttributeException>()
						.WithMessage("The attribute(s) order, value(s), or usage(s) is/are irrational.");

		[Fact]
		public void CanThrow_IrrationalAttributeValueException()
			=> ((Action)(() =>
			{
				throw new IrrationalAttributeValueException();
			})).Should().ThrowExactly<IrrationalAttributeValueException>()
						.WithMessage("An invalid value has been specified for an attribute");

		[Fact]
		public void CanThrow_InvalidDeclarationException()
			=> ((Action)(() =>
			{
				throw new InvalidDeclarationException();
			})).Should().ThrowExactly<InvalidDeclarationException>()
						.WithMessage("The class isn't defined properly");

		[Fact]
		public void CanThrow_NoDefaultConstructorException()
			=> ((Action)(() =>
			{
				throw new NoDefaultConstructorException();
			})).Should().ThrowExactly<NoDefaultConstructorException>()
						.WithMessage("There was no default constructor specified.");
	}
}