using Decorator.Attributes;

using Xunit;

namespace Decorator.Tests {

	public class Deserialization {

		private bool TestTestable<T>(T test, Message msg)
			where T : ITestable {
			test.AssertType(msg);
			Assert.True(msg.Args != null, $"Null arguments.");
			test.AssertArgs(msg.Args);

			return true;
		}

		private void Test(string type, params object[] args) {
			var msg = new Message(type, args);

			var extraData = new bool[] { false };
			Deserializer.DeserializeToEvent(this, msg, msg, extraData);

			Assert.True(extraData[0]);
		}

		[Fact(DisplayName = "Type:      Null      Value:    Null")]
		public void _1()
			=> Test(null, new object[] { null });

		[Fact(DisplayName = "Type:      Null      Value:    Non-Null")]
		public void _2()
			=> Test(null, 1234);

		[Fact(DisplayName = "Type:      Non-Null  Value:    Null")]
		public void _3()
			=> Test(NonNullType.TypeName, new object[] { null });

		[Fact(DisplayName = "Type:      Non-Null  Value:    Non-Null")]
		public void _4()
			=> Test(NonNullType.TypeName, "lorem ipsum");

		[DeserializedHandler]
		public void HandleNull(NullType nullType, Message msg, bool[] modify) {
			TestTestable(nullType, msg);
			modify[0] = true;
		}

		[DeserializedHandler]
		public void HandleNonNull(NonNullType nonNullType, Message msg, bool[] modify) {
			TestTestable(nonNullType, msg);
			modify[0] = true;
		}
	}
}