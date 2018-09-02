// ROADMAP:
// - add an attribute to specify the amount of "favor
//   points" a class gets by default
//
// - Optional & Required attributes worth different points
//
// - add [Deserializable] attribute to classes and
//   let the classes implement a method to deserialize
//   an int, uint, or more.
//
// - Required attribute by default
// - ENUM support

using System;

namespace Decorator.Tester {

	[Decorator.Attributes.Repeatable, Decorator.Attributes.Message("chat")]
	public class ChatLog {

		[Decorator.Attributes.Position(0)]
		public string Message { get; set; }

		[Decorator.Attributes.Position(1)]
		public int RandomId { get; set; }
	}

	public class Program {

		[Decorator.Attributes.DeserializedHandler]
		public static void HandleThingy(System.Collections.Generic.IEnumerable<ChatLog> example) {
			foreach (var i in example)
				Console.WriteLine("rec chatlog: " + i.Message + " - " + i.RandomId);
		}

		public static void Main() {
			var msg = Serializer.Serialize((System.Collections.Generic.IEnumerable<ChatLog>)new ChatLog[] {
				new ChatLog {
					Message = "harry was",
					RandomId = 3
				},
				new ChatLog {
					Message = "venelope was",
					RandomId = 6
				},
				new ChatLog {
					Message = "x gn giv it to yau",
					RandomId = 9
				}
			});

			if(!Deserializer.DeserializeToEvent<Program>(null, msg)) {
				Console.WriteLine("D:: eror r D:");
				Console.ReadLine();
			}

			var client1 = new Client();
			var client2 = new Client();
			var client3 = new Client();
			var server = new Server();

			client1.Join(server);
			client2.Join(server);
			client3.Join(server);

			client1.SendChat("Hello, World!");
			client2.SendChat("Hey, nice to meet you too!");

			client3.SendChat("Bye guys!");
			client3.Disconnect();

			client2.SendChat("Cya!");
			client2.Disconnect();

			client1.SendChat("Darn, I got chores to do anyways.");
			client1.Disconnect();

			Console.ReadLine();
		}
	}
}