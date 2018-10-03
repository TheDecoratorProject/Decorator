using Decorator.Attributes;

using System.Collections.Generic;

using Xunit;

namespace Decorator.Tests
{
    public class HandlerClass
    {
        public bool Invoked { get; private set; }
        public int InvokedCount { get; private set; }
        public int Items { get; private set; }

        public void IgnoredMethod(TestMessage m)
            => Assert.False(true, $"Shouldn't be invoked D:");

        [DeserializedHandler]
        public void NullHandler(NullType n)
            => Assert.True(false);

        [DeserializedHandler]
        public void TestMessageHandler(TestMessage m)
        {
			// ienumerable
			if (m.PositionOneItem == 0) return;

            Assert.Equal(1337, m.PositionOneItem);
            Invoked = true;
            InvokedCount++;
        }

        [DeserializedHandler]
        public void AnotherTestMessageHandler(TestMessage m)
		{
			// ienumerable
			if (m.PositionOneItem == 0) return;


			Assert.Equal(1337, m.PositionOneItem);
            Invoked = true;
            InvokedCount++;
        }

        [DeserializedHandler]
        public void EnumerableHandler(TestMessage[] m)
        {
            if (m.Length == 1) return;

            var c = 0;

            foreach (var i in m)
            {
                Assert.Equal("just right", i.PositionZeroItem);
                Assert.Equal(c++, i.PositionOneItem);
                Items++;
            }

            Invoked = true;
            InvokedCount++;
        }
    }

    public class OtherHandlerClass
    {
        public bool Invoked { get; private set; }
        public int InvokedCount { get; private set; }

        [DeserializedHandler]
        public void EnumerableHandler(IEnumerable<TestMessage> m)
        {
            var c = 0;
            foreach (var i in m)
            {
                Assert.Equal("floss", i.PositionZeroItem);
                Assert.Equal(c++, i.PositionOneItem);
            }

            Invoked = true;
            InvokedCount++;
        }
    }
}
