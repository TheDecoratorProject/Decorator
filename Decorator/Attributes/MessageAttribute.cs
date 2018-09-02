using System;

namespace Decorator.Attributes {

	/// <summary>
	/// Allows a class to be used as a valid Message, ready for serialization/deserialization.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class MessageAttribute : Attribute {

		public MessageAttribute(string type) => this.Type = type;

		public string Type { get; set; }
	}
}