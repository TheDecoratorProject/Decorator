using System;

namespace Decorator.Attributes {

	/// <summary>
	/// On every message property by default, but it forces the deserializer to make sure that there's a valid value for this property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class RequiredAttribute : Attribute {
	}
}