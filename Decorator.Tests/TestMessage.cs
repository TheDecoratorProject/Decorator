using Decorator.Attributes;

namespace Decorator.Tests {

	[Message("test"), Repeatable]
	public class TestMessage {

		[Position(0)]
		public string PositionZeroItem { get; set; }

		[Position(1)]
		public int PositionOneItem { get; set; }

		public float Ignored { get; set; }
	}

	[Message(null)]
	public class NullType {
	}

	[Message("rep")]
	public class NonRepeatable {

		[Position(0)]
		public int SomeValue { get; set; }
	}

	[Message("opt")]
	public class OptionalMsg {

		[Position(0), Required]
		public string RequiredString { get; set; }

		[Position(1), Optional]
		public int OptionalValue { get; set; }
	}
}