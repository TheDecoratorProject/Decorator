using System;

namespace Decorator.Attributes {

	/// <summary>Defines where in an object array this property should get it's value from.</summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class PositionAttribute : Attribute {

		public PositionAttribute(int position) => this.Position = position;

		public int Position { get; set; }
	}
}