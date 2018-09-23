using BenchmarkDotNet.Running;

using System;
using System.Threading;

namespace Decorator.Benchmarks {

	internal static class Program {

		private static void Main(string[] args) {
			ProtocolMessage.ProtocolMessageManager _pm = new ProtocolMessage.ProtocolMessageManager();

			var mre = new ManualResetEvent(false);

			for (int i = 0; i < 0b1000_0000; i++)
				new Thread(() => {
					mre.WaitOne();
					while(true)
						_pm.Convert<ProtocolMessage.Chat>(new object[] {
							5, "wew"
						});
				}).Start();

			Console.ReadLine();
			mre.Set();
			Console.WriteLine("goo");
			Console.ReadLine();

			BenchmarkRunner.Run<Decorator.Benchmarks.Benchmarks>();
			Console.ReadLine();
		}
	}
}