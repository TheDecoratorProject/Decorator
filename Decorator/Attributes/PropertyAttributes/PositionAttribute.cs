using System;

namespace Decorator.Attributes {

	/// <summary>
	/// Sets where the position of this attribute is in the message
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class PositionAttribute : Attribute {

		public PositionAttribute(int position) => this.Position = position;

		public int Position { get; set; }
	}
}