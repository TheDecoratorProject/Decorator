using System;

namespace Decorator
{
	public class DecoratorCompilerException : Exception
	{
		internal DecoratorCompilerException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}