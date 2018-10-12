using BenchmarkDotNet.Attributes;

using Decorator.Attributes;

using System;

namespace Decorator.Benchmarks
{
	[Deserialiser.Deserialisable]
	[ProtocolMessage.Message("say")]
	[Message("say")]
	public class Chat
	{
		[Deserialiser.Order(0)]
		[ProtocolMessage.Position(0), ProtocolMessage.Required]
		[Position(0), Required]
		public int PlayerId { get; set; }

		[Deserialiser.Order(1)]
		[ProtocolMessage.Position(1), ProtocolMessage.Required]
		[Position(1), Required]
		public string Message { get; set; }
	}

	public class Benchmarks
	{
		private readonly object[] args = new object[] { 5, "heyo !" };

		private BaseMessage _bm;
		private ProtocolMessage.ProtocolMessageManager _pm;

		[GlobalSetup]
		public void Setup()
		{
			_pm = new ProtocolMessage.ProtocolMessageManager();

			_bm = new BasicMessage("say", args);

			TryDecorator();
			TryProtocolMessage();
			TryDeserialiser();
		}

		[Benchmark]
		public bool TryDecorator()
			=> Decorator.Deserializer.TryDeserializeItem<Chat>(_bm, out _);

		[Benchmark]
		public void TryProtocolMessage()
			=> _pm.Convert<Chat>(args);

		[Benchmark]
		public void TryDeserialiser()
			=> Deserialiser.Deserialiser<Chat>.Deserialise(args);
	}

	/*
	public class Benchmarks
	{
		private readonly object[] _goodArgs = new object[] { 10, "hello world" };
		private readonly object[] _badArgsAt0 = new object[] { "10", "hello world" };
		private readonly object[] _badArgsAt1 = new object[] { 10, 3f };

		private BaseMessage _goodMsg;
		private BaseMessage _badMsgAt0;
		private BaseMessage _badMsgAt1;
		private BaseMessage _badType;

		private Chat _chat;

		private ProtocolMessage.ProtocolMessageManager _pm;

		private readonly Type _type = typeof(Decorator.Benchmarks.Chat);

		[GlobalSetup]
		public void Setup()
		{
			_pm = new ProtocolMessage.ProtocolMessageManager();
			_goodMsg = new BasicMessage("say", _goodArgs);
			_badMsgAt0 = new BasicMessage("say", _badArgsAt0);
			_badMsgAt1 = new BasicMessage("say", _badArgsAt1);
			_badType = new BasicMessage("sya", _goodArgs);
			_chat = new Chat {
				Message = "hello world",
				PlayerId = 10
			};

			// dry run, ensure caching in decorator is fine
			BasicDeserialize();

			for (var i = 0; i < 2; i++)
			{
				BasicDeserialize();
				ProtocolMessage();
				DeserializeWithType();
				InvalidChat_At0();
				InvalidChat_At1();
				InvalidChat_Type();
			}
		}
		[Message("demo")]
		public class DemoMessage
		{
			[Position(0), Required]
			public string SomeData { get; set; }

			[Position(1), Optional]
			public int IntegerData { get; set; }
		}

		[Message("fsat")]
		public class FlattenComplexArraysTest
		{
			[Position(0), Flatten]
			public DemoMessage[] SomeStringData { get; set; }

			public override bool Equals(object obj)
			{
				return obj is FlattenComplexArraysTest fsat &&
						fsat.SomeStringData.Equals(this.SomeStringData);
			}
		}

		[Benchmark(Description = "eee")]
		public bool UhhArrays()
			=> Deserializer.TryDeserializeItem<FlattenComplexArraysTest>(new BasicMessage("fsat", 3, "message", 1234, "more", 4567, "8778", null), out _);

		[Benchmark(Description = "Simple TryDeserialize", Baseline = true)]
		public bool BasicDeserialize()
			=> Deserializer.TryDeserializeItem<Chat>(_goodMsg, out var _);

		[Benchmark(Description = "ProtocolMessage alternative")]
		public ProtocolMessage.Chat ProtocolMessage()
			=> _pm.Convert<ProtocolMessage.Chat>(_goodArgs);

		[Benchmark(Description = "TryDeserialize with Type")]
		public bool DeserializeWithType()
			=> Deserializer.TryDeserializeItem(_type, _goodMsg, out var _);

		[Benchmark(Description = "Invalid @ 0")]
		public bool InvalidChat_At0()
			=> Deserializer.TryDeserializeItem<Chat>(_badMsgAt0, out var _);

		[Benchmark(Description = "Invalid @ 1")]
		public bool InvalidChat_At1()
			=> Deserializer.TryDeserializeItem<Chat>(_badMsgAt1, out var _);

		[Benchmark(Description = "Invalid Type")]
		public bool InvalidChat_Type()
			=> Deserializer.TryDeserializeItem<Chat>(_badType, out var _);

		[Benchmark(Description = "Deserialize Message to Method")]
		public void InvokeMethodMessage()
			=> Deserializer<Benchmarks>.InvokeMethodFromMessage(this, _goodMsg);

		[Benchmark(Description = "Deserialize Item to Method")]
		public void InvokeMethodItem()
			=> Deserializer<Benchmarks>.InvokeMethodFromItem(this, _chat);

		[DeserializedHandler]
		public void HandleItem(Chat chat)
		{
			// Allow the deserializer to discover this method
		}
	}*/
}