using BenchmarkDotNet.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Decorator.Benchmarks
{
	/// <summary>
	/// CONCLUSION:
	/// IL is faster then Expressions.
	/// </summary>
	public class FuncBenchies
	{
		private MethodInfo method => typeof(FuncBenchies).GetMethod("Method");

		private Func<object, object[], object> _invoker;
		private Func<object, object[], object> _thing;
		private Func<object, object[], object> _il;

		[GlobalSetup]
		public void Setup()
		{
			_invoker = CreateInvoker(method);
			_thing = DoThing(method);
			_il = ILWrapRefSupport(method);
		}

		[Benchmark]
		public void CreateInvokerTest() => CreateInvoker(method);

		[Benchmark]
		public void DoThingTest() => DoThing(method);

		[Benchmark]
		public void GenILTest() => ILWrapRefSupport(method);

		[Benchmark]
		public void CallInvoker() => _invoker(null, new object[] { 1234, "" });

		[Benchmark]
		public void CallThing() => _thing(null, new object[] { 1234, "" });

		[Benchmark]
		public void CallIL() => _il(null, new object[] { 1234, "" });

		[Benchmark]
		public string CallMethodWithOverhead()
		{
			// simulate some of the overhead like the others
			var obj = new object[] { 1234, "" };

			Method((int)obj[1], out var test);

			return test;
		}

		[Benchmark]
		public string CallMethodAndReturn()
		{
			Method(1234, out var res);
			return res;
		}

		[Benchmark]
		public void OnlyCallMethod() => Method(1234, out _);

		public static bool Method(int wee, out string test)
		{
			test = (wee * 20).ToString();
			return wee % 2 == 0;
		}

		public static Func<object, object[], object> CreateInvoker(MethodInfo method)
		{
			var parameters = method.GetParameters();
			var targetParam = Expression.Parameter(typeof(object), "target");
			var paramsParam = Expression.Parameter(typeof(object[]), "params");
			var resultVariable = Expression.Variable(method.ReturnType, "return");

			var returnTarget = Expression.Label(typeof(object));

			var variableDict = parameters
				.Where(x => x.ParameterType.IsByRef)
				.ToDictionary(x => x.Position,
					x => Expression.Variable(x.ParameterType.GetElementType()));

			var callParams = parameters
				.Select(p => variableDict.TryGetValue(p.Position, out var result)
					? result : (Expression)Expression.Convert(
						Expression.ArrayIndex(paramsParam,
							Expression.Constant(p.Position)), p.ParameterType));

			var beforeAssignments = variableDict
				.Select(x => Expression.Assign(x.Value,
					Expression.Convert(
						Expression.ArrayIndex(paramsParam,
							Expression.Constant(x.Key)),
						parameters[x.Key].ParameterType.GetElementType())));

			var afterAssignments = variableDict
				.Select(x => Expression.Assign(
					Expression.ArrayAccess(paramsParam,
						Expression.Constant(x.Key)),
					Expression.Convert(x.Value, typeof(object))));

			var variables = variableDict
				.Select(x => x.Value)
				.Append(resultVariable);

			var block = new List<Expression>();
			block.AddRange(beforeAssignments);
			block.Add(
				Expression.Assign(resultVariable,
					Expression.Call(
						method.IsStatic ? null : Expression.Convert(targetParam, method.DeclaringType),
						method, callParams)));
			block.AddRange(afterAssignments);
			block.Add(Expression.Label(returnTarget,
				Expression.Convert(resultVariable, typeof(object))));

			return Expression.Lambda<Func<object, object[], object>>(
				Expression.Block(variables, block),
				targetParam, paramsParam).Compile();
		}

		public static Func<object, object[], object> DoThing(MethodInfo info)
		{
			var instance = Expression.Parameter(typeof(object), "instance");
			var parameterInstruction = Expression.Parameter(typeof(object[]), "params");

			var infoParams = info.GetParameters();
			var parameters = new Expression[infoParams.Length];
			var expressions = new List<Expression>();
			var vars = new List<ParameterExpression>();
			for (int i = 0; i < parameters.Length; i++)
			{
				var t = infoParams[i].ParameterType;
				if (t.IsByRef)
				{
					var v = Expression.Variable(t.GetElementType());
					vars.Add(v);
					parameters[i] = v;
				}
				else
					parameters[i] = Expression.Convert(Expression.ArrayIndex(parameterInstruction, Expression.Constant(i)), t);
			}

			var res = Expression.Variable(typeof(object), "res");
			vars.Add(res);
			expressions.Add(res);
			var call = Expression.Call(info.IsStatic ? null : instance, info, parameters);
			expressions.Add(Expression.Assign(res, Expression.ConvertChecked(call, typeof(object))));

			for (int i = 0; i < parameters.Length; i++)
			{
				expressions.Add(Expression.Assign(Expression.ArrayAccess(parameterInstruction, Expression.Constant(i)), Expression.Convert(parameters[i], typeof(object))));
			}
			var end = Expression.Label(typeof(object));
			expressions.Add(Expression.Return(end, res, typeof(object)));
			expressions.Add(Expression.Label(end, res));

			return Expression.Lambda<Func<object, object[], object>>(Expression.Block(variables: vars, expressions: expressions), instance, parameterInstruction).Compile();
		}

		public static Func<object, object[], object> ILWrapRefSupport(MethodInfo method)
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
			var locals = new LocalBuilder[parameters.Length];

			for (var i = 0; i < parameters.Length; i++)
			{
				if (!parameters[i].IsOut)
				{
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldc_I4, i);
					il.Emit(OpCodes.Ldelem_Ref);
				}

				var paramType = parameters[i].ParameterType;
				if (paramType.IsValueType)
					il.Emit(OpCodes.Unbox_Any, paramType);
			}

			for (var i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].IsOut)
				{
					locals[i] = il.DeclareLocal(parameters[i].ParameterType.GetElementType());
					il.Emit(OpCodes.Ldloca, locals[locals.Length - 1]);
				}
			}

			il.EmitCall(method.IsStatic || method.DeclaringType.IsValueType ?
				OpCodes.Call : OpCodes.Callvirt, method, null);

			for (var idx = 0; idx < parameters.Length; ++idx)
			{
				if (parameters[idx].IsOut || parameters[idx].ParameterType.IsByRef)
				{
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldc_I4, idx);
					il.Emit(OpCodes.Ldloc, locals[idx].LocalIndex);

					if (parameters[idx].ParameterType.GetElementType().IsValueType)
						il.Emit(OpCodes.Box, parameters[idx].ParameterType.GetElementType());

					il.Emit(OpCodes.Stelem_Ref);
				}
			}

			if (method.ReturnType == null || method.ReturnType == typeof(void))
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
	}
}