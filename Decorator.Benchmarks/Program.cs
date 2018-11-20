using BenchmarkDotNet.Attributes;
using Decorator.Attributes;
using Decorator.Compiler;
using Decorator.ModuleAPI;
using Decorator.Modules;
using System;
using System.Linq;
using System.Reflection;

namespace Decorator.Benchmarks
{
	internal static class Program
	{
		private static void Main()
		{
			BenchmarkDotNet.Running.BenchmarkRunner.Run<ILPerformance>();
			Console.ReadLine();
		}
	}

	public class ILPerformance
	{
		public class Test
		{
			[Position(0), Required]
			public string StringA { get; set; }

			[Position(1), Required]
			public string StringB { get; set; }

			[Position(2), Required]
			public string StringC { get; set; }

			[Position(3), Required]
			public string StringD { get; set; }
		}

		private object[] _data;
		private ILDeserialize<Test> _il;

		public ILPerformance()
		{
			_data = new object[] { "a", "b", "c", "d" };
			_il = new Compiler<Test>().CompileDeserializeIL((i) => new Container(typeof(string), new Member((PropertyInfo)i)));

			foreach (var benchmark in GetType()
								.GetMethods()
								.Where(x => x.GetCustomAttributes(true)
												.OfType<BenchmarkAttribute>()
												.Count() > 0))
			{
				var res = (Test)benchmark.Invoke(this, null);

				if (res.StringA != "a") throw new Exception("A Des. Invalid");
				if (res.StringB != "b") throw new Exception("B Des. Invalid");
				if (res.StringC != "c") throw new Exception("C Des. Invalid");
				if (res.StringD != "d") throw new Exception("D Des. Invalid");
			}
		}

		[Benchmark]
		public Test DConverterDeserialize()
		{
			if (!DConverter<Test>.TryDeserialize(_data, out var result))
			{
				throw new Exception();
			}

			return result;
		}

		[Benchmark]
		public Test ILDeserialize()
		{
			int i = 0;
			if (!_il(_data, ref i, out var result))
			{
				throw new Exception();
			}

			return result;
		}
	}
}