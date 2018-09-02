using Decorator.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace Decorator {
	// TODO: use enums instead of out strings

	public static partial class Deserializer {

		/// <summary>
		/// Deserialize a Message, and find the appropiate DeserializedHandlerAttribute that takes the type of the deserialized message.
		/// </summary>
		/// <typeparam name="T">The class to look for methods in</typeparam>
		/// <param name="eventClass">An instance of the class. Leave it to null to find static methods</param>
		/// <param name="msg">The message to be deserializing</param>
		/// <param name="extraParams">Additional parameters that you wish to pass into the function</param>
		/// <returns>If it succeeded or not.</returns>
		public static bool DeserializeToEvent<T>(T eventClass, Message msg, params object[] extraParams)
			where T : class, new() {
			var success = false;

			// loop through every deserializable handler
			foreach (var i in ReflectionHelper.GetMethodsWithAttribute<DeserializedHandlerAttribute>(ReflectionHelper.GetTypeOf(eventClass))) {

				// get the args
				var args = i.GetParameters();

				if (args?.Length < 1) throw new CustomAttributeFormatException($"Invalid [{nameof(DeserializedHandlerAttribute)}] - must have at least one parameter");

				// get the type
				var desType = args[0].ParameterType;

				// try to convert to an IEnumerable
				var isEnumerable = InvokeIEnumerableMethod(eventClass, desType, msg, i, extraParams, out var _);

				if (!isEnumerable && 
					!desType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)) &&
					TryDeserializeGenerically(msg, desType, out var param, out var _)) {
					success = true;

					InvokeMethod(eventClass, i, param, extraParams);
				}
			}

			return success;
		}

		private static void InvokeMethod<T>(T eventClass, MethodInfo meth, object invokItm, object[] extraParams)
			where T : class {
			var invokeArgs = new object[extraParams.Length + 1];

			Array.Copy(extraParams, 0, invokeArgs, 1, extraParams.Length);

			invokeArgs[0] = invokItm;

			meth.Invoke(eventClass, invokeArgs);
		}
	}
}