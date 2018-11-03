using BenchmarkDotNet.Attributes;
using System;

namespace Decorator.Benchmarks
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			BenchmarkDotNet.Running.BenchmarkRunner.Run<Benchmarks>();
			Console.ReadLine();
		}
	}
}