using System;

namespace Decorator.Attributes {

	/// <summary>Define a class as a message. This will allow the message to be deserialized.</summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class MessageAttribute : Attribute {

		public MessageAttribute(string type) {
			this.Type = type;
		}

		public string Type { get; set; }
	}
}