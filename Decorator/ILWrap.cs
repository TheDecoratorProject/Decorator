using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Decorator {

	public delegate object ILFunc(object instance, object[] args);

	internal static class IL {
		// https://stackoverflow.com/a/7478557

		private static Type _ilfuncTypeCache = typeof(ILFunc);

		public static ILFunc ILWrap(this MethodInfo method) {
			var dm = new DynamicMethod(method.Name, typeof(object), new [] {
					typeof(object), typeof(object[])
				}, method.DeclaringType, true);
			var il = dm.GetILGenerator();

			if (!method.IsStatic) {
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Unbox_Any, method.DeclaringType);
			}
			var parameters = method.GetParameters();
			for (var i = 0; i < parameters.Length; i++) {
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldc_I4, i);
				il.Emit(OpCodes.Ldelem_Ref);
				il.Emit(OpCodes.Unbox_Any, parameters[i].ParameterType);
			}
			il.EmitCall(method.IsStatic || method.DeclaringType.IsValueType ?
				OpCodes.Call : OpCodes.Callvirt, method, null);
			if (method.ReturnType == null || method.ReturnType == typeof(void)) {
				il.Emit(OpCodes.Ldnull);
			} else if (method.ReturnType.IsValueType) {
				il.Emit(OpCodes.Box, method.ReturnType);
			}
			il.Emit(OpCodes.Ret);
			return (ILFunc)dm.CreateDelegate(_ilfuncTypeCache);
		}

		// https://stackoverflow.com/a/29133510

		public static ILFunc ILWrapRefSupport(this MethodInfo method) {
			var dm = new DynamicMethod(method.Name, typeof(object), new[] {
					typeof(object), typeof(object[])
				}, method.DeclaringType, true);
			var il = dm.GetILGenerator();

			if (!method.IsStatic) {
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Unbox_Any, method.DeclaringType);
			}

			var parameters = method.GetParameters();
			LocalBuilder[] locals = new LocalBuilder[parameters.Length];

			for (var i = 0; i < parameters.Length; i++) {

				if (!parameters[i].IsOut) {
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldc_I4, i);
					il.Emit(OpCodes.Ldelem_Ref);
				}

				var paramType = parameters[i].ParameterType;
				if (paramType.IsValueType)
					il.Emit(OpCodes.Unbox_Any, paramType);
			}

			for (var i = 0; i < parameters.Length; i++) {
				if (parameters[i].IsOut) {
					locals[i] = il.DeclareLocal(parameters[i].ParameterType.GetElementType());
					il.Emit(OpCodes.Ldloca, locals[locals.Length - 1]);
				}
			}

			il.EmitCall(method.IsStatic || method.DeclaringType.IsValueType ?
				OpCodes.Call : OpCodes.Callvirt, method, null);


			for (int idx = 0; idx < parameters.Length; ++idx) {
				if (parameters[idx].IsOut || parameters[idx].ParameterType.IsByRef) {
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldc_I4, idx);
					il.Emit(OpCodes.Ldloc, locals[idx].LocalIndex);

					if (parameters[idx].ParameterType.GetElementType().IsValueType)
						il.Emit(OpCodes.Box, parameters[idx].ParameterType.GetElementType());

					il.Emit(OpCodes.Stelem_Ref);
				}
			}


			if (method.ReturnType == null || method.ReturnType == typeof(void)) {
				il.Emit(OpCodes.Ldnull);
			} else if (method.ReturnType.IsValueType) {
				il.Emit(OpCodes.Box, method.ReturnType);
			}

			il.Emit(OpCodes.Ret);
			return (ILFunc)dm.CreateDelegate(_ilfuncTypeCache);
		}
	}
}