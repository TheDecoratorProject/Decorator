using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Decorator.Tests
{
    public class DesHandlers
    {
		[Fact]
		public void DeserializesToHandlerTestMessage() {
			var setup = Setup.GetSetup();

			setup.Deserializer.DeserializeToMethod(setup.Instance, new TestMessage {
				PositionZeroItem = "",
				PositionOneItem = 1337
			});

			Assert.True(setup.Instance.Invoked);
		}

		[Fact]
		public void DeserializeToHandlerMessage() {
			var setup = Setup.GetSetup();

			setup.Deserializer.DeserializeToMethod(setup.Instance, Setup.Correct);

			Assert.True(setup.Instance.Invoked);
		}
    }
}
