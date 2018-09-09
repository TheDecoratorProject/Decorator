using Decorator.Attributes;

using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace Decorator.Tests {

	public class HandlerClass {
		public bool Invoked { get; private set; }
		public int InvokedCount { get; private set; }

		public void IgnoredMethod(TestMessage m) {
			Assert.False(true, $"Shouldn't be invoked D:");
		}

		[DeserializedHandler]
		public void NullHandler(NullType n) {
			Assert.True(false);
		}

		[DeserializedHandler]
		public void TestMessageHandler(TestMessage m) {
			Assert.Equal(1337, m.PositionOneItem);
			this.Invoked = true;
			this.InvokedCount++;
		}

		[DeserializedHandler]
		public void AnotherTestMessageHandler(TestMessage m) {
			Assert.Equal(1337, m.PositionOneItem);
			this.Invoked = true;
			this.InvokedCount++;
		}

		[DeserializedHandler]
		public void EnumerableHandler(IEnumerable<TestMessage> m) {
			if (m.Count() == 1) return;

			var c = 0;

			foreach (var i in m) {
				Assert.Equal("just right", i.PositionZeroItem);
				Assert.Equal(c++, i.PositionOneItem);
			}

			this.Invoked = true;
			this.InvokedCount++;
		}
	}

	public class OtherHandlerClass {
		public bool Invoked { get; private set; }
		public int InvokedCount { get; private set; }

		[DeserializedHandler]
		public void EnumerableHandler(IEnumerable<TestMessage> m) {
			var c = 0;
			foreach (var i in m) {
				Assert.Equal("floss", i.PositionZeroItem);
				Assert.Equal(c++, i.PositionOneItem);
			}

			this.Invoked = true;
			this.InvokedCount++;
		}
	}
}