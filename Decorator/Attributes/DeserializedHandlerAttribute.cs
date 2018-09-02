using System;

namespace Decorator.Attributes {

	/// <summary>
	/// Mark this on a method to say that the method is valid for deserialization.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public sealed class DeserializedHandlerAttribute : Attribute {
	}
}