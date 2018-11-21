using System;

namespace Decorator.Exceptions
{
	public class DecoratorCompilerException : Exception
	{
		public DecoratorCompilerException() : base()
		{
		}

		public DecoratorCompilerException(string message) : base(message)
		{
		}

		public DecoratorCompilerException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}