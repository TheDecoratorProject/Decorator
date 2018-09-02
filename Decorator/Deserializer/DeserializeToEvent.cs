using Decorator.Attributes;

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
			where T : class {
			var success = false;

			foreach (var i in ReflectionHelper.GetMethodsWithAttribute<DeserializedHandlerAttribute>(ReflectionHelper.GetTypeOf(eventClass))) {
				var args = i.GetParameters();

				if (args?.Length < 1) throw new CustomAttributeFormatException($"Invalid [{nameof(DeserializedHandlerAttribute)}] - must have at least one parameter");

				var desType = args[0].ParameterType;

				success = DeserializeMessageToIEnumerableAndInvoke(eventClass, desType, msg, i, extraParams) || success;

				if (ReflectionHelper.TryGetAttributeOf<MessageAttribute>(desType, out var msgAttrib) &&
					TryDeserializeGenerically(msg, desType, out var param, out var _)) {
						success = true;

						// merge extraParams & the first message together

						var invokeArgs = new object[extraParams.Length + 1];
						invokeArgs[0] = param;
						for (var k = 0; k < extraParams.Length; k++)
							invokeArgs[k + 1] = extraParams[k];

						i.Invoke(eventClass, invokeArgs);
				}
			}

			return success;
		}
	}
}