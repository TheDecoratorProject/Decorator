using System;
using System.Collections.Generic;
using System.Text;

#if THIS_SHOULDNT_HAPPEN

namespace Decorator.Exceptions {
	/// <summary>If something goes wrong in the constructor, you're screwed, and you need to report the issue to github ASAP.</summary>
	/// <seealso cref="Decorator.Exceptions.DecoratorException" />
	public class ConstructorException : DecoratorException {
		internal ConstructorException(string msg) : base(msg) {

		}
	}
}

#endif