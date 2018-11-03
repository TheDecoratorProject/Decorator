using BenchmarkDotNet.Attributes;
using System;

namespace Decorator.Benchmarks
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			BenchmarkDotNet.Running.BenchmarkRunner.Run<TestSpeed>();
			Console.ReadLine();
		}
	}

	public class TestSpeed
	{
		private Action<object> _recNull;
		private Action<object> _recNonNull;

		private Action<object> _triggerOnValue;

		[Params("", null)]
		public object input;

		public TestSpeed()
		{
			_triggerOnValue = (o) =>
			{

			};

			_recNull = (o) => { };
			_recNonNull = (o) => { _triggerOnValue(o); };
		}

		[Benchmark]
		public void TriggerNull()
			=> _recNull(null);

		[Benchmark]
		public void TriggerNonNull()
			=> _recNonNull(null);

		[Benchmark]
		public void If()
		{
			if (input == null)
			{
				return;
			}

			_triggerOnValue(null);
		}
	}
}