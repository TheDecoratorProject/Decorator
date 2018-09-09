using Xunit;

namespace Decorator.Tests {

	public class CanDeserializeProperly {

		[Fact]
		public void TooShortMessage() {
			Assert.False(Setup.GetSetup().Deserializer.CanDeserialize<TestMessage>(Setup.TooShort));
		}

		[Fact]
		public void TooLongMessage() {
			Assert.False(Setup.GetSetup().Deserializer.CanDeserialize<TestMessage>(Setup.TooLong));
		}

		[Fact]
		public void InvalidBaseMessage() {
			Assert.False(Setup.GetSetup().Deserializer.CanDeserialize<TestMessage>(Setup.InvalidBase));
		}

		[Fact]
		public void IncorrectTypesMessage() {
			Assert.False(Setup.GetSetup().Deserializer.CanDeserialize<TestMessage>(Setup.IncorrectTypes));
		}

		[Fact]
		public void NullValuesMessage() {
			Assert.False(Setup.GetSetup().Deserializer.CanDeserialize<TestMessage>(Setup.NullValues));
		}

		[Fact]
		public void NullTypeMessage() {
			Assert.False(Setup.GetSetup().Deserializer.CanDeserialize<TestMessage>(Setup.NullType));
		}

		[Fact]
		public void AllNullMessage() {
			Assert.False(Setup.GetSetup().Deserializer.CanDeserialize<TestMessage>(Setup.AllNull));
		}

		[Fact]
		public void CorrectMessage() {
			Assert.True(Setup.GetSetup().Deserializer.CanDeserialize<TestMessage>(Setup.Correct));
		}

		[Fact]
		public void OnlyNullMessage() {
			Assert.True(Setup.GetSetup().Deserializer.CanDeserialize<NullType>(Setup.OnlyNullType));
		}
	}
}