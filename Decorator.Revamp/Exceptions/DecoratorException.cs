using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator.Exceptions
{
    public class DecoratorException : Exception
    {
		internal DecoratorException(string msg) : base(msg) {

		}
    }
}
