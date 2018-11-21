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
		}

		private object[] _data;
		private Test _class;
		private ILDeserialize<Test> _ilDes;
		private ILSerialize<Test> _ilSer;

		public ILPerformance()
		{
			/*
		[Fact]
		public void Deserializes()
		{
			var c = new Compiler.Compiler<TestClass>();

			var deserializer = c.CompileILDeserialize(c.Compile((i) =>
			{
				return new Container(((PropertyInfo)i).PropertyType, new Member((PropertyInfo)i));
			}));

			int l = 0;
			deserializer(new object[] { "testing 1, 2, 3" }, ref l, out var result)
				.Should().BeTrue();

			result.MyProperty.Should().Be("testing 1, 2, 3");
		}

		[Fact]
		public void Serializes()
		{
			var c = new Compiler.Compiler<TestClass>();

			var serializer = c.CompileILSerialize(c.Compile((i) =>
			{
				return new Container(((PropertyInfo)i).PropertyType, new Member((PropertyInfo)i));
			}));

			var result = serializer(new TestClass
			{
				MyProperty = "testing 1, 2, 3"
			});*/

			var c = new Compiler<Test>();

			var modules = c.Compile((i) =>
			{
				return new Container(((PropertyInfo)i).PropertyType, new Member((PropertyInfo)i));
			});

			_ilDes = c.CompileILDeserialize(modules);
			_ilSer = c.CompileILSerialize(modules);

			_class = new Test
			{
				StringA = "a"
			};

			_data = DConverter<Test>.Serialize(_class);

			foreach (var benchmark in GetType()
								.GetMethods()
								.Where(x => x.GetCustomAttributes(true)
												.OfType<BenchmarkAttribute>()
												.Count() > 0))
			{
				benchmark.Invoke(this, null);
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
			if (!_ilDes(_data, ref i, out var result))
			{
				throw new Exception();
			}

			return result;
		}

		[Benchmark]
		public object[] DConverterSerialize()
		{
			return DConverter<Test>.Serialize(_class);
		}

		[Benchmark]
		public object[] ILSerialize()
		{
			return _ilSer(_class);
		}
	}
}
 