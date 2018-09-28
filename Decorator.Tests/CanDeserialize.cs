using Xunit;

namespace Decorator.Tests
{
	public class CanDeserializeProperly
	{
		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void TooShortMessage()
			=> Assert.False(Decorator.Deserializer.TryDeserializeItem<TestMessage>(Setup.TooShort, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void TooLongMessage()
			=> Assert.False(Decorator.Deserializer.TryDeserializeItem<TestMessage>(Setup.TooLong, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void InvalidBaseMessage()
			=> Assert.False(Decorator.Deserializer.TryDeserializeItem<TestMessage>(Setup.InvalidBase, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void IncorrectTypesMessage()
			=> Assert.False(Decorator.Deserializer.TryDeserializeItem<TestMessage>(Setup.IncorrectTypes, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void NullValuesMessage()
			=> Assert.False(Decorator.Deserializer.TryDeserializeItem<TestMessage>(Setup.NullValues, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void NullTypeMessage()
			=> Assert.False(Decorator.Deserializer.TryDeserializeItem<TestMessage>(Setup.NullType, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void AllNullMessage()
			=> Assert.False(Decorator.Deserializer.TryDeserializeItem<TestMessage>(Setup.AllNull, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void CorrectMessage()
			=> Assert.True(Decorator.Deserializer.TryDeserializeItem<TestMessage>(Setup.Correct, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "CanDeserialize")]
		public void OnlyNullMessage()
			=> Assert.True(Decorator.Deserializer.TryDeserializeItem<NullType>(Setup.OnlyNullType, out var _));
	}
}