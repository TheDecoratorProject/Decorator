using System;

namespace Deserialiser
{
	public class NotDeserialisableException : Exception
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

	public class InvalidDeserialisationInfoException : Exception
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

	public class InvalidTypeException : Exception
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

	public class IncorrectValueException : Exception
	{
		public IncorrectValueException()
		{
		}

		public IncorrectValueException(string message) : base(message)
		{
		}

		public IncorrectValueException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}