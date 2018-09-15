using BenchmarkDotNet.Running;
using System;

namespace Decorator.Benchmarks {

	internal static class Program {

		private static void Main(string[] args) {
			BenchmarkRunner.Run<Decorator.Benchmarks.Benchmarks>();
			Console.ReadLine();
		}
	}
}