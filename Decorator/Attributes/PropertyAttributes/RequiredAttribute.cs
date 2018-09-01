using System;
using System.Reflection;

namespace Decorator.Attributes {

	/// <summary>Tells the deserializer that this property is required.</summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class RequiredAttribute : Attribute {

		public RequiredAttribute() {
		}
	}
}