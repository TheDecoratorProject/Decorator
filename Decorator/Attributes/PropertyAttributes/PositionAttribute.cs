using System;
using System.Reflection;

namespace Decorator.Attributes {

	/// <summary>Defines where in an object array this property should get it's value from.</summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class PositionAttribute : Attribute {

		public PositionAttribute(int position) {
			this.Position = position;
		}

		public int Position { get; set; }

		public bool CheckRequirements<T>(PropertyInfo propInfo, Message msg, T item, PositionAttribute pos) {
			/*
			if (msg.Args == null) throw new NullReferenceException($"Detected a position attribute, but the message arguments are null.");
			if (msg.Args.Length <= pos.Position) throw new MessageException($"There aren't enough args in the message to match the position attribute.");
			if (propInfo.PropertyType != msg.Args[pos.Position].GetType()) throw new MessageException($"The message type in the args isn't equal to the property's value.");
			*/

			return true;
		}
	}
}