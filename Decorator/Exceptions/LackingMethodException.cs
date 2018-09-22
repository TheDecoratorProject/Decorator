using System;

namespace Decorator.Exceptions {

	/// <summary>Gets thrown whenever there are no methods in a class that belong to type T</summary>
	/// <seealso cref="Decorator.Exceptions.DecoratorException" />
	public sealed class LackingMethodsException : DecoratorException {

		public LackingMethodsException(Type t) : base($"Unable to find any methods that are associated with [{t}]") {
		}
	}
}