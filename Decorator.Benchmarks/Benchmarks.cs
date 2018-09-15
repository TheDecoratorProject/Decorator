﻿using BenchmarkDotNet.Attributes;
using Decorator.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Decorator.Benchmarks {

	[Message("say")]
	public class Chat {
		[Position(0), Required]
		public int PlayerId { get; set; }

		[Position(1), Required]
		public string Message { get; set; }

		public Chat() {
		}
	}

	public class Benchmarks {

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
		public void Setup() {
			this._pm = new ProtocolMessage.ProtocolMessageManager();
			this._goodMsg = new BasicMessage("say", this._goodArgs);
			this._badMsgAt0 = new BasicMessage("say", this._badArgsAt0);
			this._badMsgAt1 = new BasicMessage("say", this._badArgsAt1);
			this._badType = new BasicMessage("sya", this._goodArgs);
			this._chat = new Chat {
				Message = "hello world",
				PlayerId = 10
			};

			// dry run, ensure caching in decorator is fine

			for (var i = 0; i < 2; i++) {
				BasicDeserialize();
				ProtocolMessage();
				DeserializeWithType();
				InvalidChat_At0();
				InvalidChat_At1();
				InvalidChat_Type();
			}
		}

		[Benchmark(Description = "Simple TryDeserialize", Baseline = true)]
		public bool BasicDeserialize()
			=> Deserializer.TryDeserialize<Chat>(this._goodMsg, out var _);

		[Benchmark(Description = "ProtocolMessage alternative")]
		public ProtocolMessage.Chat ProtocolMessage()
			=> this._pm.Convert<ProtocolMessage.Chat>(this._goodArgs);

		[Benchmark(Description = "TryDeserialize with Type")]
		public bool DeserializeWithType()
			=> Deserializer.GruntWorker.TryDeserializeType(this._type, this._goodMsg, out var _);

		[Benchmark(Description = "Invalid @ 0")]
		public bool InvalidChat_At0()
			=> Deserializer.TryDeserialize<Chat>(this._badMsgAt0, out var _);

		[Benchmark(Description = "Invalid @ 1")]
		public bool InvalidChat_At1()
			=> Deserializer.TryDeserialize<Chat>(this._badMsgAt1, out var _);

		[Benchmark(Description = "Invalid Type")]
		public bool InvalidChat_Type()
			=> Deserializer.TryDeserialize<Chat>(this._badType, out var _);

		[Benchmark(Description = "Deserialize Message to Method")]
		public void InvokeMethodMessage()
			=> Deserializer<Benchmarks>.DeserializeMessageToMethod(this, this._goodMsg);

		[Benchmark(Description = "Deserialize Item to Method")]
		public void InvokeMethodItem()
			=> Deserializer<Benchmarks>.DeserializeItemToMethod(this, this._chat);

		[DeserializedHandler]
		public void HandleItem(Chat chat) {

		}
	}
}