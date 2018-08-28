using System;
using Decorator.Attributes;

namespace Decorator.Tester {
	[Message("ping")]
	public class Ping {

		[Position(0)]
		[Required]
		public int Pong { get; set; }

		public override string ToString()
			=> $"[{nameof(Ping)}] {this.Pong}";
	}

	[Message("ping")]
	public class Ping2 {

		[Position(0)]
		[Required]
		public int Pong2 { get; set; }

		public override string ToString()
			=> $"[{nameof(Ping2)}] {this.Pong2}";
	}

	[Message("ping")]
	public class PingWithMessage {

		[Position(0)]
		[Required]
		public int Pong { get; set; }

		[Position(1)]
		[Required]
		public string Message { get; set; }

		public override string ToString()
			=> $"[{nameof(PingWithMessage)}] {this.Pong}: {this.Message}";
	}

	class Program {
		static void Main(string[] args) {
			var ping = new Ping {
				Pong = 55
			};

			var pingwm = new PingWithMessage {
				Pong = 1337,
				Message = "testing"
			};

			var msg = Serializer.Serialize(ping);
			var msg2 = Serializer.Serialize(pingwm);

			Console.WriteLine(msg);
			Console.WriteLine(msg2);

			msg = new Message("ping", "eight");

			Get<Ping>(msg.Type, msg.Args);
			Get<PingWithMessage>(msg2.Type, msg2.Args);
			
			Deserializer.DeserializeToEvent<Program>(null, msg);

			Deserializer.DeserializeToEvent<Program>(new Program(), msg2);

			Deserializer.DeserializeToEvent<Program>(null, msg2);

			//Deserializer.DeserializeToEvent<Program>(new Program(), msg);

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
		public static void OnGetPingStatic(Ping p) {
			Console.WriteLine($"Static Ping! {p}");
		}

		[DeserializedHandler]
		public void OnGetPing(Ping p) {
			Console.WriteLine($"Ping! {p}");
		}

		[DeserializedHandler]
		public void OnGetPing(Ping2 p) {
			Console.WriteLine($"Ping! {p}");
		}

		[DeserializedHandler]
		public static void OnGetPWMStatic(PingWithMessage p) {
			Console.WriteLine($"Static PingWithmessage! {p}");
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
