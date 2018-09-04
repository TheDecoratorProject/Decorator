using Decorator.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Decorator.Tests {

	[Message("test")]
	public class Test1 {

		[Position(0)]
		public int IntValue { get; set; }
	}

	[Message("test")]
	public class Test2 {

		[Position(0)]
		public string StringValue { get; set; }
	}

	[Message("setT")]
	public class Test3 {

		[Position(0)]
		public int IntValue { get; set; }
	}

	[Message("setT")]
	public class Test4 {

		[Position(0)]
		public string StringValue { get; set; }
	}

	public class Tests {

		[Fact]
		public void A() {
			const int intVal = 1337;

			var msg = Serializer.Serialize(new Test1 {
				IntValue = intVal
			});

			Assert.Equal(intVal, msg.Args[0]);
			Assert.Equal("test", msg.Type);
		}

		[Fact]
		public void B() {
			const string strVal = "ttest";

			var msg = Serializer.Serialize(new Test2 {
				StringValue = strVal
			});

			Assert.Equal(strVal, msg.Args[0]);
			Assert.Equal("test", msg.Type);
		}

		[Fact]
		public void C() {
			const int intVal = 1337;

			var msg = Serializer.Serialize(new Test1 {
				IntValue = intVal
			});

			Assert.Equal(1, Deserializer.DeserializeToEvent<CEvent>(null, msg));
		}

		[Fact]
		public void D() {
			const string strVal = "ttest";

			var msg = Serializer.Serialize(new Test2 {
				StringValue = strVal
			});

			Assert.Equal(1, Deserializer.DeserializeToEvent<DEvent>(null, msg));
		}

		[Fact]
		public void E() {
			const int intVal = 1337;

			var msg = Serializer.Serialize(new Test1 {
				IntValue = intVal
			});

			Assert.Equal(2, Deserializer.DeserializeToEvent<EEvent>(null, msg));
		}

		[Fact]
		public void F() {
			const string strVal = "ttest";

			var msg = Serializer.Serialize(new Test2 {
				StringValue = strVal
			});

			Assert.Equal(3, Deserializer.DeserializeToEvent<EEvent>(null, msg));
		}

		[Fact]
		public void G() {
			const int intVal = 1337;

			var msg = Serializer.Serialize(new Test3 {
				IntValue = intVal
			});

			Assert.Throws<AggregateException>(() => Deserializer.DeserializeToEvent<EEvent>(null, msg));
		}

		[Fact]
		public void H() {
			const string strVal = "ttest";

			var msg = Serializer.Serialize(new Test4 {
				StringValue = strVal
			});

			Assert.Throws<AggregateException>(() => Deserializer.DeserializeToEvent<EEvent>(null, msg));
		}

		[Fact]
		public void I() {
			const int intVal = 1337;

			var msg = Serializer.Serialize(new Test1 {
				IntValue = intVal
			});

			Assert.Equal(1, Deserializer.DeserializeToEvent<FEvent>(null, msg));
		}

		[Fact]
		public void J() {
			const int intVal = 1337;

			var msg = Serializer.Serialize(new Test3 {
				IntValue = intVal
			});

			Assert.Equal(2, Deserializer.DeserializeToEvent<FEvent>(null, msg));
		}
	}

	public class CEvent {
		[DeserializedHandler]
		public static void HandleTest1(Test1 test) {

		}
	}

	public class DEvent {
		[DeserializedHandler]
		public static void HandleTest2(Test2 test) {

		}
	}

	public class EEvent {
		[DeserializedHandler]
		public static void HandleTest1_1(Test1 test) {

		}

		[DeserializedHandler]
		public static void HandleTest1_2(Test1 test) {

		}

		[DeserializedHandler]
		public static void HandleTest2_1(Test2 test) {

		}

		[DeserializedHandler]
		public static void HandleTest2_2(Test2 test) {

		}

		[DeserializedHandler]
		public static void HandleTest2_3(Test2 test) {

		}
	}
	
	public class FEvent {
		[DeserializedHandler]
		public static void HandleTest1(Test1 test) {

		}

		[DeserializedHandler]
		public static void HandleTest3(Test3 test) {

		}

		[DeserializedHandler]
		public static void HandleTest3_(Test3 test) {

		}
	}
}