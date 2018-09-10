using Decorator.Exceptions;

using System.Collections.Generic;

using Xunit;

namespace Decorator.Tests {

	public class Deserialize {

		private static void Verify(BaseMessage m, TestMessage t) {
			Assert.Equal("test", m.Type);
			Assert.Equal(m.Arguments[0], t.PositionZeroItem);
			Assert.Equal(m.Arguments[1], t.PositionOneItem);
		}

		private static void AttemptDeserialize(BaseMessage msg) {
			var result = Deserializer.Deserialize<TestMessage>(msg);

			Verify(msg, result);
		}

		private static void AttemptDeserializeRepeated(BaseMessage msg, int repeatAmt) {
			var args = new List<object>();

			for (var i = 0; i < repeatAmt; i++) {
				msg.Arguments[1] = i;
				args.AddRange(msg.Arguments);
			}

			var result = Deserializer.DeserializeRepeats<TestMessage>(new BasicMessage("test", args.ToArray()));

			var c = 0;
			foreach (var i in result) {
				Assert.Equal("just right", i.PositionZeroItem);
				Assert.Equal(c, i.PositionOneItem);

				c++;
			}
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "Deserialization")]
		public void DeserializesValuesCorrectly()
			=> AttemptDeserialize(Setup.Correct);

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "Deserialization")]
		public void DeserializesIncorrectValuesCorrectly()
			=> Assert.Throws<InvalidDeserializationAttemptException>(() => AttemptDeserialize(Setup.IncorrectTypes));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "Deserialization")]
		public void DeserializesRepeatsCorrectly()
			=> AttemptDeserializeRepeated(Setup.Correct, 3);

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "Deserialization")]
		public void DeserializesIncorrectRepeatsCorrectly()
			=> Assert.Throws<InvalidDeserializationAttemptException>(() => AttemptDeserializeRepeated(Setup.IncorrectTypes, 3));
	}
}