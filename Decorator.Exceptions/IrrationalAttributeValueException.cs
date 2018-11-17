﻿using System;

namespace Decorator.Exceptions
{
	public class IrrationalAttributeValueException : IrrationalAttributeException
	{
		public IrrationalAttributeValueException() : this(ExceptionManager.IrrationalAttributeValueDefault)
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