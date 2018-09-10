using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Decorator.Tests
{
    public class SerializeTests {
		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanSerialize")]
		public void SerializeExample() {
			Assert.Equal(new BasicMessage("test", "frog", 8), Serializer<TestMessage>.Serialize(new TestMessage {
				PositionOneItem = 8,
				PositionZeroItem = "frog"
			}));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanSerialize")]
		public void SerializeEmpty() {
			Assert.Equal(new BasicMessage(null), Serializer<NullType>.Serialize(new NullType()));
		}
	}
}
