using System;
using System.Reflection;

namespace Decorator.Attributes {

	/// <summary>Tells the deserializer that this property is required.</summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class RequiredAttribute : Attribute, IPropertyAttributeBase {

		public RequiredAttribute() {
		}

		public bool CheckRequirements<T>(PropertyInfo propInfo, Message msg, T item, PositionAttribute pos) {
			if (msg.Args == null) throw new MessageException($"The arguments are null on a required attribute.");
			if (msg.Args.Length <= pos.Position) throw new MessageException($"The message's arg length doesn't meet the position of this required attribute");

			return true;
		}
	}
}