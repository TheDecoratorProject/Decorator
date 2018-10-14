using System;
using System.Runtime.Serialization;

namespace Decorator.Exceptions
{
	/// <summary>Gets thrown whenever an attribute is missing that should be on it</summary>
	/// <seealso cref="Decorator.Exceptions.DecoratorException" />
	public sealed class InvalidMessageException : DecoratorException
	{
		public InvalidMessageException(Type t) : base($"This class {t} is an invalid message for some reason. Figure it out. (sorry)")
		{
		}
	}
}