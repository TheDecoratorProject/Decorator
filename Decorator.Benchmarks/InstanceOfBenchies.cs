using BenchmarkDotNet.Attributes;

using System;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Decorator.Benchmarks
{
	internal static class ExpressionInstanceOf<T>
	{
		public static T Create()
			=> Constructor();

		private static readonly Func<T> Constructor = GenConstructor();

		public static Func<T> GenConstructor()
			=> Expression.Lambda<Func<T>>(
					Expression.New(typeof(T))
				).Compile();
	}

	internal static class ILInstanceOf<T>
	{
		public static T Create()
			=> Constructor();

		private static readonly Func<T> Constructor = GenConstructor();

		public static Func<T> GenConstructor()
		{
			var typeT = typeof(T);

			var dm = new DynamicMethod("", typeT, null);
			var il = dm.GetILGenerator();

			il.Emit(OpCodes.Newobj, typeT.GetConstructors()[0]);
			il.Emit(OpCodes.Ret);

			return (Func<T>)dm.CreateDelegate(typeof(Func<T>));
		}
	}

	public class InstanceOfBenchies
	{
		public class SampleClass { }

		[Benchmark]
		public void GenerateExpressionCtor()
			=> ExpressionInstanceOf<SampleClass>.GenConstructor();

		[Benchmark]
		public void GenerateILCtor()
			=> ILInstanceOf<SampleClass>.GenConstructor();

		[Benchmark]
		public void ExpressionInstance()
			=> ExpressionInstanceOf<SampleClass>.Create();

		[Benchmark]
		public void ILInstance()
			=> ILInstanceOf<SampleClass>.Create();
	}
}