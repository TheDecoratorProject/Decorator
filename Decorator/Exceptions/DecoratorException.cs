using System;

namespace Decorator.Exceptions {

	public class DecoratorException : Exception {

		internal DecoratorException(string msg) : base(msg) {
		}
	}
}