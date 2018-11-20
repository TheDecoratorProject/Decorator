using System;

namespace Decorator.Exceptions
{
	public class DecoratorCompilerException : Exception
	{
		public DecoratorCompilerException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}