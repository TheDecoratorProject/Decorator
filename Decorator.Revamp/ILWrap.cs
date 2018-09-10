using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Decorator {

	internal static class IL {
		// https://stackoverflow.com/a/7478557

		public static Func<object, object[], object> Wrap(MethodInfo method) {
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
			return (Func<object, object[], object>)dm.CreateDelegate(typeof(Func<object, object[], object>));
		}
	}
}