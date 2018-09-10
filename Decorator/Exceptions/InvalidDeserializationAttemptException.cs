using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator.Exceptions
{
    public class InvalidDeserializationAttemptException : DecoratorException
    {
		public InvalidDeserializationAttemptException() : base("There was an attempt to deserialize, but it is invalid to do so.") {

		}
    }
}
