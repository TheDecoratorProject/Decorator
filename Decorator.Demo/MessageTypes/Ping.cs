using Decorator.Attributes;

namespace Decorator.Demo.MessageTypes
{
	[Message("ping")]
	public class Ping
	{
		[Position(0)]
		public int IntegerValue { get; set; }

		public override string ToString()
			=> $"[{nameof(Ping)}]: {nameof(IntegerValue)}: {IntegerValue}";
	}
}