// ROADMAP:
// - add an attribute to specify the amount of "favor
//   points" a class gets by default
//
// - Optional & Required attributes worth different points
//
// - add [Deserializable] attribute to classes and
//   let the classes implement a method to deserialize
//   an int, uint, or more.

namespace Decorator.Tester {

	internal class Program {

		private static void Main(string[] args) {
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

			System.Threading.Thread.Sleep(1000);
		}
	}
}