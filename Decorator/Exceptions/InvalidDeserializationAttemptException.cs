namespace Decorator.Exceptions {

	/// <summary>Gets thrown whenever it is invalid to deserialize a message to a specific type (the message is formatted incorrectly).</summary>
	/// <seealso cref="Decorator.Exceptions.DecoratorException" />
	public class InvalidDeserializationAttemptException : DecoratorException {

		public InvalidDeserializationAttemptException() : base("There was an attempt to deserialize, but it is invalid to do so.") {
		}
	}
}