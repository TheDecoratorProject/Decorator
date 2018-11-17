﻿using System;

namespace Decorator
{
	public class InvalidDeclarationException : DecoratorCompilerException
	{
		public InvalidDeclarationException() : this(ExceptionManager.InvalidDeclarationDefault)
		{
		}

		public InvalidDeclarationException(string message) : this(message, null)
		{
		}

		public InvalidDeclarationException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}