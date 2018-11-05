using System;

namespace Decorator.Benchmarks
{
	internal static class Program
	{
		private static void Main()
		{
			BenchmarkDotNet.Running.BenchmarkRunner.Run<Benchmarks>();
			Console.ReadLine();
		}
	}
}