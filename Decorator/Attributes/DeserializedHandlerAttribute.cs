using System;

namespace Decorator.Attributes {

	/// <summary>When a message is successfully deserialized, it will use these attributes to find the proper handler for it.</summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public sealed class DeserializedHandlerAttribute : Attribute {
	}
}