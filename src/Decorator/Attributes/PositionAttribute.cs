using System;

namespace Decorator.Attributes
{
	public sealed class PositionAttribute : Attribute
	{
		public PositionAttribute(int position) => Position = position;

		public int Position { get; set; }
	}
}