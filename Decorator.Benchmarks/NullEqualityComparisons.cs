using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator.Benchmarks
{
	public class NullEqualityComparisons
	{
		[Params(true, false)]
		public bool UseNullValue { get; set; }

		private object SomeObject { get; set; }

		[GlobalSetup]
		public void Setup()
		{
			if (UseNullValue)
			{
				SomeObject = null;
			} else
			{
				SomeObject = new object[] { 123, "456", 789.0f, 101112u };
			}
		}

		[Benchmark]
		public bool IsNull()
		{
			return SomeObject == null;
		}

		[Benchmark]
		[Arguments(null)]
		[Arguments(789.0f)]
		public bool Equals(object cmp)
		{
			return SomeObject == cmp;
		}

		[Benchmark]
		[Arguments(null)]
		[Arguments(789.0f)]
		public bool IsNullOptimization(object inp)
		{
			return inp == null ?
					SomeObject == null
					: inp == SomeObject;
		}
	}
}
