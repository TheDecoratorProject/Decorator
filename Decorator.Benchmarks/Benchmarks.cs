using BenchmarkDotNet.Attributes;
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

		private object[] _args = new object[] { 10, "hello world" };
		private BaseMessage _bm;
		private ProtocolMessage.ProtocolMessageManager _pm;

		private Type _type = typeof(Decorator.Benchmarks.Chat);

		[GlobalSetup]
		public void Setup() {
			this._pm = new ProtocolMessage.ProtocolMessageManager();
			this._bm = new BasicMessage("say", this._args);

			// dry run, ensure caching in decorator is fine

			for(var i = 0; i < 5; i++) {
				Decorator();
				ProtocolMessage();
				DecoratorType();
			}
		}
		
		[Benchmark(Baseline = true)]
		public ProtocolMessage.Chat ProtocolMessage() {
			return this._pm.Convert<ProtocolMessage.Chat>(this._args);
		}
		
		[Benchmark]
		public bool Decorator() {
			return Deserializer.TryDeserialize<Chat>(this._bm, out var _);
		}

		[Benchmark(Description = "Decorator via Type")]
		public bool DecoratorType() {
			return Deserializer.GruntWorker.TryDeserializeType(this._type, this._bm, out var _);
		}
	}
}