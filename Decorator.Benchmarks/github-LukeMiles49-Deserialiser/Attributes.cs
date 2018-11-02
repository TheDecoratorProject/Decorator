using System;

namespace Deserialiser
{
	[AttributeUsage(AttributeTargets.Property)]
	public class OrderAttribute : Attribute
	{
		public readonly double position;
		public readonly bool recurse;

		public OrderAttribute(double position, bool recurse = false)
		{
			this.position = position;
			this.recurse = recurse;
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ConstAttribute : OrderAttribute
	{
		public readonly object value;

		public ConstAttribute(double position, object value, bool recurse = false) : base(position, recurse) => this.value = value;
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class DeserialisableAttribute : Attribute
	{
	}
}