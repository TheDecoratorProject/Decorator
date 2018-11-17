﻿using System;

namespace Decorator
{
	public class NoDefaultConstructorException : InvalidDeclarationException
	{
		public NoDefaultConstructorException() : this(ExceptionManager.NoDefaultConstructorDefault)
		{
		}

		public NoDefaultConstructorException(string message) : this(message, null)
		{
		}

		public NoDefaultConstructorException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}