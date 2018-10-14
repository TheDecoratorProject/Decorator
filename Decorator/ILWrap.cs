using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Decorator
{
	internal delegate object ILFunc(object instance, object[] args);

	// magic class that nobody really knows what it does :D

	internal static class IL
	{
		// https://stackoverflow.com/a/7478557

		private static readonly Type _ilfuncTypeCache = typeof(ILFunc);
		private static readonly Type _object = typeof(object);

		public static Action<object, object> GetSetMethodByExpression(this PropertyInfo propertyInfo)
		{
			var prop = propertyInfo.GetSetMethod();

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

		// https://stackoverflow.com/questions/20491162/create-expression-function-from-methodinfo-with-unknown-signature

		public static Action<object, object> GetSingleInvokable(this MethodInfo method)
		{
			var paramExprs = GetParamExpr(method);
			var paramTypes = GetParamTypes(method, paramExprs);
			var instanceExp = Expression.Convert(paramExprs[0], method.DeclaringType);
			var call = Expression.Call(instanceExp, method, paramTypes);
			return (Action<object, object>)Expression.Lambda(call, paramExprs).Compile();
		}

		private static List<ParameterExpression> GetParamExpr(MethodInfo method)
		{
			var list = new List<ParameterExpression> {
				Expression.Parameter(_object, "obj")
			};
			list.AddRange(Array.ConvertAll(method.GetParameters(), input => Expression.Parameter(_object)));
			return list;
		}

		private static List<Expression> GetParamTypes(MethodInfo method, List<ParameterExpression> inList)
		{
			var list = new List<Expression>();
			var methParams = method.GetParameters();
			list.AddRange(
				// Skip the first item as this is the object on which the method is called.
				inList.Skip(1).Select(
					input => Expression.Convert(
						input,
						methParams[inList.IndexOf(input) - 1].ParameterType)));
			return list;
		}

		public static ILFunc ILWrap(this MethodInfo method)
		{
			var dm = new DynamicMethod(method.Name, _object, new[] {
					_object, typeof(object[])
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
			return (ILFunc)dm.CreateDelegate(_ilfuncTypeCache);
		}

		// https://stackoverflow.com/a/29133510

		public static ILFunc ILWrapRefSupport(this MethodInfo method)
		{
			var dm = new DynamicMethod(method.Name, _object, new[] {
					_object, typeof(object[])
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

			if (method.ReturnType is null || method.ReturnType == typeof(void))
			{
				il.Emit(OpCodes.Ldnull);
			}
			else if (method.ReturnType.IsValueType)
			{
				il.Emit(OpCodes.Box, method.ReturnType);
			}

			il.Emit(OpCodes.Ret);
			return (ILFunc)dm.CreateDelegate(_ilfuncTypeCache);
		}
	}
}