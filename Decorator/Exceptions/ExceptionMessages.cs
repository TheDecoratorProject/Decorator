using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
	internal static class ExceptionMessages
	{
		public const string InvalidDeclarationDefault = "The class isn't defined properly";

		public const string NoDefaultConstructorDefault = "There was no default constructor specified.";

		public const string BrokenAttributePairingDefault = "There should be another attribute paired with the current attribute, however it is not there";

		public const string IrrationalAttributeDefault = "The attribute(s) order, value(s), or usage(s) is/are irrational.";

		public const string IrrationalAttributeValueDefault = "An invalid value has been specified for an attribute";

		public static string BrokenAttributePairing()
			=> "";

		public static string InvalidDeclaration()
			=> "";

		public static string IrrationalAttribute()
			=> "";

		public static string NoDefaultConstructor()
			=> "";
	}
}
