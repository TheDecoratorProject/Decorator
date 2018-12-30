using System;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class PositionAttribute : Attribute
	{
		public PositionAttribute(uint position) => Position = position;

		public uint Position { get; }
	}
}