using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator.Exceptions
{
    public class LackingMethodsException : DecoratorException
    {
		public LackingMethodsException(Type t) : base($"Unable to find any methods that are associated with [{t}]") {

		}
    }
}
