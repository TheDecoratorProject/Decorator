using System;

namespace Deserialiser
{
	class NotDeserialisableException : Exception
	{
		public NotDeserialisableException() { }
		public NotDeserialisableException(string message) : base(message) { }
		public NotDeserialisableException(string message, Exception inner) : base(message, inner) { }
	}

	class InvalidDeserialisationInfoException : Exception
	{
		public InvalidDeserialisationInfoException() { }
		public InvalidDeserialisationInfoException(string message) : base(message) { }
		public InvalidDeserialisationInfoException(string message, Exception inner) : base(message, inner) { }
	}

	class InvalidTypeException : Exception
	{
		public InvalidTypeException() { }
		public InvalidTypeException(string message) : base(message) { }
		public InvalidTypeException(string message, Exception inner) : base(message, inner) { }
	}
}
