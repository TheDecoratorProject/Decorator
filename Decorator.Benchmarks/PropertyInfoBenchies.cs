using BenchmarkDotNet.Attributes;

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Decorator.Benchmarks
{
	/// <summary>
	/// CONCLUSION:
	/// IL is faster then Expressions.
	/// </summary>
	public class PropertyInfoBenchies
	{
		public int TestProperty { get; set; }
		private PropertyInfo prop;
		private readonly MethodInfo setMethod;

		private readonly Action<object, object> expressSet;
		private readonly Func<object, object[], object> ilSet;

		private readonly Action<object, object> ilsetA;

		private readonly object[] data = new object[] { 1234 };
		private readonly int _data = 1234;

		public PropertyInfoBenchies()
		{
			prop = typeof(PropertyInfoBenchies).GetProperty(nameof(TestProperty));
			setMethod = prop.GetSetMethod();
			/*
			expressSet = GetSetMethodByExpression(prop, setMethod);*/
			ilSet = ILWrap(setMethod);
			ilsetA = ILSet(setMethod);

			TestProperty = 1;
			ilsetA(this, 20);

			Console.WriteLine(TestProperty);
		}

		[Benchmark]
		public Action<object, object> GenerateExpression() => GetSetMethodByExpression(prop, setMethod);

		[Benchmark]
		public Func<object, object[], object> GenerateIL() => ILWrap(setMethod);

		[Benchmark]
		public Action<object, object> GenerateSetAction() => ILSet(setMethod);

		[Benchmark]
		public void ExpressionSet() => expressSet(this, 1234);

		[Benchmark]
		public void ILSetWithAllocation() => ilSet(this, new object[] { 1234 });

		[Benchmark]
		public void ILSet() => ilSet(this, data);

		[Benchmark]
		public void PropILSet() => ilsetA(this, _data);

		public static Action<object, object> ILSet(MethodInfo prop)
		{
			var dm = new DynamicMethod(prop.Name, null, new Type[] {
				typeof(object), typeof(object)
			}, prop.DeclaringType, true);

			var il = dm.GetILGenerator();

			il.Emit(OpCodes.Ldarg_0);

			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Unbox_Any, prop.GetParameters()[0].ParameterType);

			il.EmitCall(
				prop.IsStatic || prop.DeclaringType.IsValueType ?
					OpCodes.Call
					: OpCodes.Callvirt,

				prop,
				null);

			il.Emit(OpCodes.Ret);

			return (Action<object, object>)dm.CreateDelegate(typeof(Action<object, object>));
		}

		public static Func<object, object[], object> ILWrap(MethodInfo method)
		{
			var dm = new DynamicMethod(method.Name, typeof(object), new[] {
					typeof(object), typeof(object[])
				}, method.DeclaringType, true);
			var il = dm.GetILGenerator();

			if (!method.IsStatic)
			{
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Unbox_Any, method.DeclaringType);
			}
			var parameters = method.GetParameters();
			for (var i = 0; i < parameters.Length; i++)
			{
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldc_I4, i);
				il.Emit(OpCodes.Ldelem_Ref);
				il.Emit(OpCodes.Unbox_Any, parameters[i].ParameterType);
			}
			il.EmitCall(method.IsStatic || method.DeclaringType.IsValueType ?
				OpCodes.Call : OpCodes.Callvirt, method, null);
			if (method.ReturnType is null || method.ReturnType == typeof(void))
			{
				il.Emit(OpCodes.Ldnull);
			}
			else if (method.ReturnType.IsValueType)
			{
				il.Emit(OpCodes.Box, method.ReturnType);
			}
			il.Emit(OpCodes.Ret);
			return (Func<object, object[], object>)dm.CreateDelegate(typeof(Func<object, object[], object>));
		}

		public static Action<object, object> GetSetMethodByExpression(PropertyInfo propertyInfo, MethodInfo setMethodInfo)
		{
			var _obj = typeof(object);

			var instance = Expression.Parameter(_obj, "instance");
			var value = Expression.Parameter(_obj, "value");
			var instanceCast = (!(propertyInfo.DeclaringType).GetTypeInfo().IsValueType) ? Expression.TypeAs(instance, propertyInfo.DeclaringType) : Expression.Convert(instance, propertyInfo.DeclaringType);
			var valueCast = (!(propertyInfo.PropertyType).GetTypeInfo().IsValueType) ? Expression.TypeAs(value, propertyInfo.PropertyType) : Expression.Convert(value, propertyInfo.PropertyType);

			return Expression.Lambda<Action<object, object>>(Expression.Call(instanceCast, setMethodInfo, valueCast), new ParameterExpression[] { instance, value }).Compile();
		}
	}
}