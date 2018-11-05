using System;

namespace Decorator
{
	public class IrrationalAttributeException : DecoratorCompilerException
	{
		public IrrationalAttributeException() : this(ExceptionManager.IrrationalAttributeDefault)
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