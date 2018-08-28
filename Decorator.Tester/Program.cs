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

	// FOR REFERENCE ONLY

	/*
	public class DemoMessage {
		public string Type { get; set; }

		public object[] Stuff { get; set; }
	}

	public class DemoMessageConsulter : IMessageConsulter {
		private DemoMessage _msg;

		public DemoMessageConsulter(DemoMessage msg)
			=> this._msg = msg;

		public bool IsNull()
			=> this._msg == null;

		public string GetMessageType()
			=> this._msg.Type;

		public int GetItemCount()
			=> this._msg.Stuff == null ? 0 : this._msg.Stuff.Length;

		public object GetItemAt(int index)
			=> this._msg.Stuff[index];

		public Type GetRawMessageType()
			=> typeof(DemoMessage); // note this also is fine: this._msg.GetType();
	}

    class Program
    {
		[Message("init")]
		public class Init {

			[MessagePlace(0)]
			public string BotUsername { get; set; }

			[MessagePlace(1)]
			public int BotId { get; set; }

			public override string ToString()
				=> $"{nameof(BotUsername)}: {BotUsername}\n{nameof(BotId)}: {BotId}";
		}

		[Message("say")]
		public class ChatMessage {
			[MessagePlace(0)]
			public string FirstMessage { get; set; }

			public override string ToString()
				=> $"[{nameof(ChatMessage)}] {nameof(FirstMessage)}: {FirstMessage}";
		}

		[Message("say")]
		public class ImportantChat {

			[MessagePlace(0)]
			public string FirstMessage { get; set; }

			[MessagePlace(1)]
			public int MessageType { get; set; }

			public override string ToString()
				=> $"[{nameof(ImportantChat)}] {nameof(FirstMessage)}: {FirstMessage}\n{nameof(MessageType)}: {MessageType}";
		}

		static void Main(string[] args) {
			if (Decorator.Parser.TryDeserializeGeneric(new DemoMessageConsulter(new DemoMessage {
				Type = "say",
				Stuff = new object[] { "henlo" }
			}), out var init)) {
				Console.WriteLine(init);
			} else Console.WriteLine("Couldn't des.");
			if (Decorator.Parser.TryDeserializeGeneric(new DemoMessageConsulter(new DemoMessage {
				Type = "say",
				Stuff = new object[] { "hewnlo", 1337 }
			}), out var ini2t)) {
				Console.WriteLine(ini2t);
			} else Console.WriteLine($"Couldn't des. {ini2t == null} {init == null}");
			Console.ReadLine();
        }
    }*/
}
