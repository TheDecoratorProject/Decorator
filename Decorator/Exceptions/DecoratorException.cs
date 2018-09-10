using System;

namespace Decorator.Exceptions {

	/// <summary>A generic exception for anything that happens in Decorator</summary>
	/// <seealso cref="System.Exception" />
	public class DecoratorException : Exception {

		internal DecoratorException(string msg) : base(msg) {
		}
	}
}