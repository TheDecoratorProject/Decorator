using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
	public class NoDefaultConstructorException : InvalidDeclarationException
	{
		public NoDefaultConstructorException() : this(ExceptionMessages.NoDefaultConstructorDefault)
		{
		}

		public NoDefaultConstructorException(string message) : this(message, null)
		{
		}

		public NoDefaultConstructorException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
