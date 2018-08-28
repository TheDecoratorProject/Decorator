using System;
using Decorator.Attributes;

namespace Decorator.Tester {
	[Message("ping")]
	public class Ping {

		[Position(0)]
		public int Pong { get; set; }

		public override string ToString()
			=> $"[{nameof(Ping)}] {this.Pong}";
	}

	[Message("ping")]
	public class PingWithMessage {

		[Position(0)]
		public int Pong { get; set; }

		[Position(1)]
		public string Message { get; set; }

		public override string ToString()
			=> $"[{nameof(PingWithMessage)}] {this.Pong}: {this.Message}";
	}

	class Program {
		static void Main(string[] args) {
			var ping = new Ping {
				Pong = 1337
			};

			var msg = Serializer.Serialize(ping);

			Console.WriteLine(msg);

			Get<Ping>(msg.Type, msg.Args);

			Deserializer.DeserializeToEvent<Program>(new Program(), msg);

			/*
			Get<Ping>("ping", 1234);
			Get<PingWithMessage>("ping", 1234, "k");

			Get<Ping>("pig");

			Deserializer.DeserializeToEvent<Program>(new Program(), new Message("ping", 1234));
			Deserializer.DeserializeToEvent<Program>(new Program(), new Message("ping", 1234, "k"));
			*/

			Console.ReadLine();
		}

		[DeserializedHandler]
		public void OnGetPing(Ping p) {
			Console.WriteLine($"Ping! {p}");
		}

		[DeserializedHandler]
		public void OnGetPWM(PingWithMessage p) {
			Console.WriteLine($"PingWithmessage! {p}");
		}

		static T Get<T>(string type, params object[] args) {
			var msg = new Message(type, args);

			Console.WriteLine();

			try {
				var res = Deserializer.Deserialize<T>(msg);
				if (res == null) Console.WriteLine("null");
				else Console.WriteLine(res.ToString());
				return res;
			} catch (Exception ex) {
				Console.WriteLine($"{ex.Message} {ex.StackTrace}");
				return default(T);
			}
		}
	}
}
