using System;

namespace Decorator
{
	public sealed class PositionAttribute : Attribute
	{
		public PositionAttribute(int position) => Position = position;

		public int Position { get; set; }
	}
}