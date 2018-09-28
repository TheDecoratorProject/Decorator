using System;

namespace Decorator.Attributes
{
	/// <summary>
	/// Makes the item optional.
	/// If there are not enough args, or it's passed null, or passed a type that doesn't match the target type, deserialization won't stop, it'll continue.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class OptionalAttribute : Attribute
	{
	}
}