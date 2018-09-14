using Xunit;

namespace Decorator.Tests {

	public class CanDeserializeProperly {

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void TooShortMessage()
			=> Assert.False(Deserializer.TryDeserialize<TestMessage>(Setup.TooShort, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void TooLongMessage()
			=> Assert.True(Deserializer.TryDeserialize<TestMessage>(Setup.TooLong, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void InvalidBaseMessage()
			=> Assert.False(Deserializer.TryDeserialize<TestMessage>(Setup.InvalidBase, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void IncorrectTypesMessage()
			=> Assert.False(Deserializer.TryDeserialize<TestMessage>(Setup.IncorrectTypes, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void NullValuesMessage()
			=> Assert.False(Deserializer.TryDeserialize<TestMessage>(Setup.NullValues, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void NullTypeMessage()
			=> Assert.False(Deserializer.TryDeserialize<TestMessage>(Setup.NullType, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void AllNullMessage()
			=> Assert.False(Deserializer.TryDeserialize<TestMessage>(Setup.AllNull, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void CorrectMessage()
			=> Assert.True(Deserializer.TryDeserialize<TestMessage>(Setup.Correct, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void OnlyNullMessage()
			=> Assert.True(Deserializer.TryDeserialize<NullType>(Setup.OnlyNullType, out var _));
	}
}