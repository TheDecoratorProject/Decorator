using Decorator.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Decorator.Tests
{

    public class Deserialize
    {
		private static void Verify(Message m, TestMessage t) {
			Assert.Equal("test", m.Type);
			Assert.Equal(m.Arguments[0], t.PositionZeroItem);
			Assert.Equal(m.Arguments[1], t.PositionOneItem);
		}

		private static void AttemptDeserialize(Message msg) {
			var setup = Setup.GetSetup();

			var result = setup.Deserializer.Deserialize<TestMessage>(msg);

			Verify(msg, result);
		}

		private static void AttemptDeserializeRepeated(Message msg, int repeatAmt) {
			var setup = Setup.GetSetup();

			var args = new List<object>();

			for (int i = 0; i < repeatAmt; i++) {
				msg.Arguments[1] = i;
				args.AddRange(msg.Arguments);
			}

			var result = setup.Deserializer.DeserializeRepeats<TestMessage>(new MessageImplementation("test", args.ToArray()));

			var c = 0;
			foreach(var i in result) {
				Assert.Equal("just right", i.PositionZeroItem);
				Assert.Equal(c, i.PositionOneItem);

				c++;
			}
		}

		[Fact]
		public void DeserializesValuesCorrectly()
			=> AttemptDeserialize(Setup.Correct);

		[Fact]
		public void DeserializesIncorrectValuesCorrectly()
			=> Assert.Throws<DecoratorException>(() => AttemptDeserialize(Setup.IncorrectTypes));

		[Fact]
		public void DeserializesRepeatsCorrectly()
			=> AttemptDeserializeRepeated(Setup.Correct, 3);

		[Fact]
		public void DeserializesIncorrectRepeatsCorrectly()
			=> Assert.Throws<DecoratorException>(() => AttemptDeserializeRepeated(Setup.IncorrectTypes, 3));
	}
}
