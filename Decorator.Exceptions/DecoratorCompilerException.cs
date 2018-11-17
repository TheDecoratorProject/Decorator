using System;

namespace Decorator.Exceptions
{
	public class DecoratorCompilerException : Exception
	{
		internal DecoratorCompilerException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}