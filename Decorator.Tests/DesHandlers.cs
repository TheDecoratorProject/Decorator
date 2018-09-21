﻿using Decorator.Exceptions;
using System.Collections.Generic;

using Xunit;

namespace Decorator.Tests {

	public class DesHandlers {

		[Fact]
		[Trait("Category", "HandlerDeserialization")]
		public void DesLotsLotsLOTS() {
			var instance = new HandlerClass();
			for (var i = 0; i < 100_000; i++)
				Deserializer<HandlerClass>.DeserializeMessageToMethod(instance, Setup.Correct);
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void DeserializesToHandlerTestMessage() {
			var instance = new HandlerClass();

			Deserializer<HandlerClass>.DeserializeItemToMethod(instance, new TestMessage {
				PositionZeroItem = "",
				PositionOneItem = 1337
			});

			Assert.True(instance.Invoked);
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void DeserializeToHandlerMessage() {
			var instance = new HandlerClass();

			Deserializer<HandlerClass>.DeserializeMessageToMethod(instance, Setup.Correct);

			Assert.True(instance.Invoked);
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void DeserializesEnumerable() {
			var instance = new HandlerClass();

			var args = new List<object>();

			var msg = Setup.Correct;

			// 4 is arbitrary here
			for (var i = 0; i < 4; i++) {
				msg.Arguments[1] = i;
				args.AddRange(msg.Arguments);
			}

			Assert.True(Deserializer.TryDeserializeItems<TestMessage>(msg, out var _));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void DoesntDeserializesNonEnumerable() {
			var instance = new HandlerClass();

			var args = new List<object>();

			var msg = Setup.NonRepeatable;

			// 4 is arbitrary here
			for (var i = 0; i < 4; i++) {
				msg.Arguments[0] = i;
				args.AddRange(msg.Arguments);
			}

			var t = args.ToArray();

			Assert.False(Deserializer.TryDeserializeItems<NonRepeatable>(new BasicMessage("rep", t), out var _));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void NonExistantMethod() {
			var instance = new HandlerClass();

			Assert.Throws<LackingMethodsException>(() => Deserializer<HandlerClass>.DeserializeItemToMethod(instance, new NonExistant {
				AAA = "roses are red, violets are blue, this is a string comment, woopdy doo"
			}));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", "HandlerDeserialization")]
		public void NeedsAttributes() {
			Assert.False(
				Deserializer.TryDeserializeItem<NeedsAttribute>(new BasicMessage("wew lad", "there be humans"), out var _)
			);
		}
	}
}