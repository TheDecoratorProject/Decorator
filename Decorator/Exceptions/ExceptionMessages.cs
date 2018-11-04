using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
	internal static class ExceptionMessages
	{
		public const string InvalidDeclarationDefault = "The class isn't defined properly";

		public const string NoDefaultConstructorDefault = "There was no default constructor specified.";

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
