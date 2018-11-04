using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
	public class InvalidDeclarationException : DecoratorCompilerException
	{
		public InvalidDeclarationException() : this(ExceptionMessages.InvalidDeclarationDefault)
		{
		}

		public InvalidDeclarationException(string message) : this(message, null)
		{
		}

		public InvalidDeclarationException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
