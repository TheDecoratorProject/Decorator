using BenchmarkDotNet.Attributes;
using Decorator.Attributes;
using System;

namespace Decorator.Benchmarks {

	[Message("say")]
	public class Chat {
		[Position(0), Required]
		public int PlayerId { get; set; }

		[Position(1), Required]
		public string Message { get; set; }

		[Position(2), Optional]
		public int Colour { get; set; } = 28;

		public Chat() {
		}
	}

	public class Benchmarks {

		private object[] _args = new object[] { 10, "hello world", 5 };
		private BaseMessage _bm;
		private ProtocolMessage.ProtocolMessageManager _pm;

		[GlobalSetup]
		public void Setup() {
			this._pm = new ProtocolMessage.ProtocolMessageManager();
			this._bm = new BasicMessage("say", this._args);

			// dry run, ensure caching in decorator is fine

			for(var i = 0; i < 5; i++) {
				Decorator();
				ProtocolMessage();
			}
		}

		[Benchmark(Baseline = true)]
		public void Decorator() {
			var res = Deserializer.Deserialize<Decorator.Benchmarks.Chat>(this._bm);
		}

		[Benchmark]
		public void ProtocolMessage() {
			var res = this._pm.Convert<ProtocolMessage.Chat>(this._args);
		}
	}
}