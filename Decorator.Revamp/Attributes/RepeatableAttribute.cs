using System;

namespace Decorator.Attributes {

	/// <summary>
	/// Allows a message to be repeated, serialized, and deserialized to/from an IEnumerable of the message.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class RepeatableAttribute : Attribute {
	}
}