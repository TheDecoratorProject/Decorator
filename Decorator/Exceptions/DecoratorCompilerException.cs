using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
	public class DecoratorCompilerException : Exception
	{
		internal DecoratorCompilerException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
