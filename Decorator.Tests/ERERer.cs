using Decorator.Attributes;

using System;

using Xunit;

namespace Decorator.Tests {

	[Message("base")]
	[ArgumentLimit(1)]
	public class BaseClass {

		[Position(0)]
		public string TestString { get; set; }
	}

	[Message("child")]
	public class ChildClass : BaseClass {
	}

	public class Teststs {

		[Fact]
		public void A() {
			var msg = Serializer.Serialize(new BaseClass {
				TestString = "anaibf"
			});

			Assert.Equal(msg.Type, "base");
		}

		[Fact]
		public void B() {
			var msg = Serializer.Serialize(new ChildClass {
				TestString = "anaibf"
			});

			Assert.Equal(msg.Type, "child");
		}

		[Fact]
		public void C() {
			var msg = Serializer.Serialize(new BaseClass {
				TestString = "anaibf"
			});

			Assert.Equal(1, Deserializer.DeserializeToEvent<AClass>(null, msg));
		}

		[Fact]
		public void D() {
			var msg = Serializer.Serialize(new ChildClass {
				TestString = "anaibf"
			});

			Assert.Equal(2, Deserializer.DeserializeToEvent<AClass>(null, msg));
		}

		[Fact]
		public void E() {
			var msg = new Message("base", null);

			Assert.Throws<AggregateException>(() => Deserializer.DeserializeToEvent<AClass>(null, msg));
		}

		[Fact]
		public void F() {
			var msg = new Message("child", null);

			Assert.Throws<AggregateException>(() => Deserializer.DeserializeToEvent<AClass>(null, msg));
		}

		[Fact]
		public void G() {
			var msg = new Message("base", new object[] { });

			Assert.Throws<AggregateException>(() => Deserializer.DeserializeToEvent<AClass>(null, msg));
		}

		[Fact]
		public void H() {
			var msg = new Message("child", new object[] { });

			Assert.Throws<AggregateException>(() => Deserializer.DeserializeToEvent<AClass>(null, msg));
		}

		[Fact]
		public void I() {
			var msg = new Message("base", new object[] { null });

			Assert.Throws<AggregateException>(() => Deserializer.DeserializeToEvent<AClass>(null, msg));
		}

		[Fact]
		public void J() {
			var msg = new Message("child", new object[] { null });

			Assert.Throws<AggregateException>(() => Deserializer.DeserializeToEvent<AClass>(null, msg));
		}

		[Fact]
		public void K() {
			var msg = new Message("base", new object[] { "anaibf", "dont" });

			Assert.Throws<AggregateException>(() => Deserializer.DeserializeToEvent<AClass>(null, msg));
		}

		[Fact]
		public void L() {
			var msg = new Message("child", new object[] { "anaibf", "dont" });

			Assert.Throws<AggregateException>(() => Deserializer.DeserializeToEvent<AClass>(null, msg));
		}
	}

	public class AClass {

		[DeserializedHandler]
		public static void BC(BaseClass bc) { }

		[DeserializedHandler]
		public static void CC(ChildClass bc) { }

		[DeserializedHandler]
		public static void CCC(ChildClass bc) { }
	}
}