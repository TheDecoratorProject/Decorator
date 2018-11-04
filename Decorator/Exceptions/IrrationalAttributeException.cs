using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
	public class IrrationalAttributeException : DecoratorCompilerException
	{
		public IrrationalAttributeException() : this(ExceptionMessages.IrrationalAttributeDefault)
		{
		}

		public IrrationalAttributeException(string message) : this(message, null)
		{
		}

		public IrrationalAttributeException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
