using System;

namespace Decorator
{
	internal static class ExceptionManager
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

		public static IrrationalAttributeValueException GetIrrationalAttributeValue<T>(Type onType, object value, string comment)
			where T : Attribute
			=> new IrrationalAttributeValueException($"The attribute \"{typeof(T)}\" on type \"{onType}\" was given an invalid value \"{value}\"\r\n{comment}");

		public static BrokenAttributePairingException GetBrokenAttributePairing<T>(Type declaringType, string memberName, string comment)
			where T : Attribute
			=> new BrokenAttributePairingException($"The member \"{memberName}\" in {declaringType} with attribute \"{typeof(T)}\" is missing an attribute to be paired with it.\r\n{comment}");

		public static IrrationalAttributeException GetIrrationalAttribute(string comment)
			=> new IrrationalAttributeException(comment);

		public static NoDefaultConstructorException GetNoDefaultConstructor()
			=> new NoDefaultConstructorException();
	}
}