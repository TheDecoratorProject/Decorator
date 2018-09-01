using Decorator.Attributes;

namespace Decorator.Tester.MessageTypes {

	[Message("ping")]
	public class Ping {

		[Position(0)]
		public int IntegerValue { get; set; }

		public override string ToString()
			=> $"[{nameof(Ping)}]: {nameof(this.IntegerValue)}: {this.IntegerValue}";
	}
}