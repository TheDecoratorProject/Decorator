using System;

namespace Decorator.Tester
{
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
    }
}
