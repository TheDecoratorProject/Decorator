using System;

namespace Decorator.Exceptions
{
	// TODO: why are the other overloads not available

	public class DecoratorCompilerException : Exception
	{
		public DecoratorCompilerException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}