using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
	public class IrrationalAttributeValueException : IrrationalAttributeException
	{
		public IrrationalAttributeValueException() : this("An invalid value has been specified for an attribute")
		{
		}

		public IrrationalAttributeValueException(string message) : this(message, null)
		{
		}

		public IrrationalAttributeValueException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
