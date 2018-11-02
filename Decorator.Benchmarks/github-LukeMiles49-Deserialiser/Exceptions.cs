using System;

namespace Deserialiser
{
	internal class NotDeserialisableException : Exception
	{
		public NotDeserialisableException()
		{
		}

		public NotDeserialisableException(string message) : base(message)
		{
		}

		public NotDeserialisableException(string message, Exception inner) : base(message, inner)
		{
		}
	}

	internal class InvalidDeserialisationInfoException : Exception
	{
		public InvalidDeserialisationInfoException()
		{
		}

		public InvalidDeserialisationInfoException(string message) : base(message)
		{
		}

		public InvalidDeserialisationInfoException(string message, Exception inner) : base(message, inner)
		{
		}
	}

	internal class InvalidTypeException : Exception
	{
		public InvalidTypeException()
		{
		}

		public InvalidTypeException(string message) : base(message)
		{
		}

		public InvalidTypeException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}