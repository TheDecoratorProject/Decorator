using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using Deserialiser;

using System;

namespace Decorator.Benchmarks
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			BenchmarkRunner.Run<Benchmarks>();
			Console.ReadLine();
		}
	}

	public class Benchmarks
	{
		[Deserialisable]
		public class SampleClass
		{
			[Position(0), Required]
			[Order(0)]
			public string StringProperty { get; set; }

			[Position(1), Required]
			[Order(1)]
			public int IntegerValue { get; set; }

			[Position(2), Required]
			[Order(2)]
			public double DoubleValue { get; set; }
		}

		private readonly object[] _data;

		public Benchmarks()
		{
			_data = new object[] { "string", 1337, 515d };

			// prime the pump
			Decorator();
			Deserialiser();
		}

		[Benchmark]
		public bool Decorator()
			=> DDecorator<SampleClass>.TryDeserialize(_data, out var _);

		[Benchmark]
		public SampleClass Deserialiser()
			=> Deserialiser<SampleClass>.Deserialise(_data);
	}
}