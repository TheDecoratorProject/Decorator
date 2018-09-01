using Decorator.Attributes;

using System;
using System.Reflection;

namespace Decorator {

	// TODO: use enums instead of strings

	public static class Deserializer {

		public static bool DeserializeToEvent<T>(T eventClass, Message msg, params object[] extraParams)
			where T : class {
			var success = false;

			foreach (var i in ReflectionHelper.GetDeserializableHandlers(ReflectionHelper.GetTypeOf(eventClass))) {
				var args = i.GetParameters();

				if (args?.Length < 1) throw new CustomAttributeFormatException($"Invalid [{nameof(DeserializedHandlerAttribute)}] - must have at least one parameter");

				if (TryDeserializeGenerically(msg, args[0].ParameterType, out var param, out var _)) {
					success = true;

					// merge extraParams & the first message together

					var invokeArgs = new object[extraParams.Length + 1];
					invokeArgs[0] = param;
					for (int k = 0; k < extraParams.Length; k++)
						invokeArgs[k + 1] = extraParams[k];

					i.Invoke(eventClass, invokeArgs);
				}
			}

			return success;
		}

		public static bool TryDeserialize<T>(Message msg, out T res, out string failErrMsg)
			where T : class, new() {
			// ensure some basic things

			res = default;
			failErrMsg = default;

			ReflectionHelper.CheckNull(msg, nameof(msg));

			var msgAttrib = ReflectionHelper.EnsureAttributeGet<MessageAttribute, T>();
			if (msgAttrib.Type != msg.Type) return OneLinerFail("The base message types aren't equal.", out failErrMsg);

			// if there's a limit on the amount of arguments set for the item

			if (ReflectionHelper.TryGetAttributeOf<ArgumentLimitAttribute>(typeof(T), out var limit))
				if (msg?.Args?.Length > limit.ArgLimit)
					return OneLinerFail($"Surpassed the maximum amount of args bound by the {nameof(ArgumentLimitAttribute)}", out failErrMsg);

			// loop through every property

			res = Activator.CreateInstance<T>();

			foreach (var i in typeof(T).GetProperties())

				// if it has a position
				if (ReflectionHelper.TryGetAttributeOf<PositionAttribute>(i, out var posAttrib))

					// if there's an argument in the message for it
					if (posAttrib.Position >= 0 && msg.Args?.Length > posAttrib.Position &&
						i.PropertyType.IsAssignableFrom(ReflectionHelper.GetTypeOf(msg?.Args?[posAttrib.Position]))) {
						i.SetValue(res, msg.Args[posAttrib.Position]);
					} else {
						// if there's no optional attribute, or there's a required attribute
						if (!ReflectionHelper.TryGetAttributeOf<OptionalAttribute>(i, out var _) ||
							ReflectionHelper.TryGetAttributeOf<RequiredAttribute>(i, out var __))

							// yell at 'em
							return OneLinerFail($"Unable to set {i.Name}", out failErrMsg);
					}

			return true;
		}

		public static bool TryDeserializeGenerically(Message msg, Type desType, out object res, out string failErrMsg) {
			var args = new object[] { msg, null, null };

			var generic = typeof(Deserializer).GetMethod(nameof(TryDeserialize), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(desType);
			var gmo = (bool)generic.Invoke(null, args);

			res = args[1];
			failErrMsg = (string)args[2];

			return gmo;
		}

		private static bool OneLinerFail(string amt, out string failErrMsg) {
			failErrMsg = amt;
			return false;
		}
	}
}