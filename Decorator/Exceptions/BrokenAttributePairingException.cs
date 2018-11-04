using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
	public class BrokenAttributePairingException : IrrationalAttributeException
	{
		public BrokenAttributePairingException() : this(ExceptionMessages.BrokenAttributePairingDefault)
		{
		}

		public BrokenAttributePairingException(string message) : this(message, null)
		{
		}

		public BrokenAttributePairingException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
