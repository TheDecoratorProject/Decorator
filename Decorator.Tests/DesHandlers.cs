using Decorator.Exceptions;

using System;
using System.Collections.Generic;

using Xunit;

namespace Decorator.Tests
{
	public class DesHandlers
	{
		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void DeserializesToHandlerTestMessage()
		{
			var instance = new HandlerClass();

			Deserializer<HandlerClass>.InvokeMethodFromItem(instance, new TestMessage {
				PositionZeroItem = "",
				PositionOneItem = 1337
			});

			Assert.True(instance.Invoked);
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void DeserializeToHandlerMessage()
		{
			var instance = new HandlerClass();

			Deserializer<HandlerClass>.InvokeMethodFromMessage(instance, Setup.Correct);

			Assert.True(instance.Invoked);
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void DeserializesEnumerable()
		{
			var instance = new HandlerClass();

			var args = new List<object>();

			var msg = Setup.Correct;

			// 4 is arbitrary here
			for (var i = 0; i < 4; i++)
			{
				msg.Arguments[1] = i;
				args.AddRange(msg.Arguments);
			}

			Assert.True(Decorator.Deserializer.TryDeserializeItems<TestMessage>(new BasicMessage("test", args.ToArray()), out var _));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void DeserializesAndInvokesIEnumerable()
		{
			var instance = new HandlerClass();

			var args = new List<object>();

			var msg = Setup.Correct;

			// 4 is arbitrary here
			for (var i = 0; i < 4; i++)
			{
				msg.Arguments[1] = i;
				args.AddRange(msg.Arguments);
			}

			Deserializer<HandlerClass>.InvokeMethodFromMessage(instance, new BasicMessage("test", args.ToArray()));

			Assert.True(instance.Invoked);
			Assert.Equal(4, instance.Items);
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void DoesntDeserializesNonEnumerable()
		{
			var instance = new HandlerClass();

			var args = new List<object>();

			var msg = Setup.NonRepeatable;

			// 4 is arbitrary here
			for (var i = 0; i < 4; i++)
			{
				msg.Arguments[0] = i;
				args.AddRange(msg.Arguments);
			}

			var t = args.ToArray();

			Assert.False(Decorator.Deserializer.TryDeserializeItems<NonRepeatable>(new BasicMessage("rep", t), out var _));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void NonExistantMethod()
		{
			var instance = new HandlerClass();

			Assert.Throws<LackingMethodsException>(() => Deserializer<HandlerClass>.InvokeMethodFromItem(instance, new NonExistant {
				AAA = "roses are red, violets are blue, this is a string comment, woopdy doo"
			}));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void NeedsAttributes()
		{
			Assert.False(
				Decorator.Deserializer.TryDeserializeItem<NeedsAttribute>(new BasicMessage("wew lad", "there be humans"), out var _)
			);
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void TryDeserializeItemDefaults() => Assert.Throws<ArgumentNullException>(() => Decorator.Deserializer.TryDeserializeItem<TestMessage>(default, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void TryDeserializeItemsDefaults() => Assert.Throws<ArgumentNullException>(() => Decorator.Deserializer.TryDeserializeItems<TestMessage>(default, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void TryDeserializeItemDefaultType() => Assert.Throws<ArgumentNullException>(() => Decorator.Deserializer.TryDeserializeItem(default, default, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void TryDeserializeItemsDefaultType() => Assert.Throws<ArgumentNullException>(() => Decorator.Deserializer.TryDeserializeItems(default, default, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void TryDeserializeItemDefaultMessage() => Assert.Throws<ArgumentNullException>(() => Decorator.Deserializer.TryDeserializeItem(typeof(TestMessage), default, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void TryDeserializeItemsDefaultMessage() => Assert.Throws<ArgumentNullException>(() => Decorator.Deserializer.TryDeserializeItems(typeof(TestMessage), default, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void NeedsAttributesType()
		{
			Assert.False(
				Decorator.Deserializer.TryDeserializeItem(typeof(NeedsAttribute), new BasicMessage("wew lad", "there be humans"), out var _)
			);
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void TryDeserializeItemDefaultsType() => Assert.Throws<ArgumentNullException>(() => Decorator.Deserializer.TryDeserializeItem(typeof(TestMessage), default, out var _));

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void TryDeserializeItemsDefaultsType() => Assert.Throws<ArgumentNullException>(() => Decorator.Deserializer.TryDeserializeItems(typeof(TestMessage), default, out var _));
	}
}