using BenchmarkDotNet.Attributes;
using System;

namespace Decorator.Benchmarks
{
	static class Program
    {
		static void Main(string[] args)
		{
			BenchmarkDotNet.Running.BenchmarkRunner.Run<Benchmarks>();
			Console.ReadLine();
        }
    }
}
