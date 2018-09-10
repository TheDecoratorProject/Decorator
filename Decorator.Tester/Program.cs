namespace Decorator.Tester {

	internal static class Program {

		public static void Main() {
			for (var x = 0; x < 1000; x++) {
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
			}
		}
	}
}