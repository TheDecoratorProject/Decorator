using System;
using System.Reflection;

namespace Decorator.Attributes {

	/// <summary>Tells the deserializer that this property is required.</summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class OptionalAttribute : Attribute, IPropertyAttributeBase {

		public OptionalAttribute() {
		}

		public bool CheckRequirements<T>(PropertyInfo propInfo, Message msg, T item, PositionAttribute pos) {
			return
				msg == null &&
				msg.Args == null &&
				msg.Args.Length <= pos.Position &&
				msg.Args[pos.Position] != null;
		}
	}
}