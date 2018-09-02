using Decorator.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator {
	// TODO: use enums instead of strings

	public static class Deserializer {

		public static bool DeserializeToEvent<T>(T eventClass, Message msg, params object[] extraParams)
			where T : class {
			var success = false;

			foreach (var i in ReflectionHelper.GetMethodsWithAttribute<DeserializedHandlerAttribute>(ReflectionHelper.GetTypeOf(eventClass))) {
				var args = i.GetParameters();

				if (args?.Length < 1) throw new CustomAttributeFormatException($"Invalid [{nameof(DeserializedHandlerAttribute)}] - must have at least one parameter");

				var desType = args[0].ParameterType;


				//TODO: this is super gay, refactor it smh

				// if it's an IEnumerable<> as a param
				if (desType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable))) {
					var genArg = desType.GenericTypeArguments[0];
					var genMsgAttrib = ReflectionHelper.EnsureAttributeGet<MessageAttribute>(genArg);

					if (ReflectionHelper.TryGetAttributeOf<RepeatableAttribute>(genArg, out var _)) {

						// get the largest position of the attribute
						//TODO: check int.MaxValue
						var largest = ReflectionHelper.GetLargestPositionAttribute(genArg);

						// ensure that the repeat amount is fine
						if (msg.Args.Length % (largest + 1) != 0) return false;

						var itms = new List<object>();

						for (var k = 0; k < msg.Args.Length / (largest + 1); k++) {
							var msgItms = new List<object>();

							for (var j = 0; j <= largest; j++) {
								msgItms.Add(msg.Args[(k * (largest + 1)) + j]);
							}

							var resMsg = new Message(genMsgAttrib.Type, msgItms.ToArray());

							if (TryDeserializeGenerically(resMsg, genArg, out var result, out var __)) itms.Add(result);
							else {
								//TODO: holy gosh i need refactoring
								itms = null;
								break;
							}
						}

						if (itms == null) break;

						var resItms = GenericChangeEnumerable(itms, genArg);

						success = true;

						var invokeArgs = new object[extraParams.Length + 1];
						invokeArgs[0] = resItms;
						for (var k = 0; k < extraParams.Length; k++)
							invokeArgs[k + 1] = extraParams[k];

						i.Invoke(eventClass, invokeArgs);
					}
				}

				if (ReflectionHelper.TryGetAttributeOf<MessageAttribute>(desType, out var msgAttrib)) {
					if (TryDeserializeGenerically(msg, desType, out var param, out var _)) {
						success = true;

						// merge extraParams & the first message together

						var invokeArgs = new object[extraParams.Length + 1];
						invokeArgs[0] = param;
						for (var k = 0; k < extraParams.Length; k++)
							invokeArgs[k + 1] = extraParams[k];

						i.Invoke(eventClass, invokeArgs);
					}
				}
			}

			return success;
		}

		private static object GenericChangeEnumerable(IEnumerable<object> itms, Type conv) {
			var generic = typeof(Deserializer).GetMethod(nameof(ChangeEnumerable), BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(conv);
			var gmo = generic.Invoke(null, new object[] { itms });

			return gmo;
		}

		private static IEnumerable<T> ChangeEnumerable<T>(IEnumerable<object> ine) {
			var objs = new List<T>();

			foreach (var i in ine) {
				objs.Add((T)Convert.ChangeType(i, typeof(T)));
			}

			return objs;
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

			if (ReflectionHelper.TryGetAttributeOf<ArgumentLimitAttribute>(typeof(T), out var limit) &&
				msg?.Args?.Length > limit.ArgLimit)
				return OneLinerFail($"Surpassed the maximum amount of args bound by the {nameof(ArgumentLimitAttribute)}", out failErrMsg);

			// loop through every property

			res = Activator.CreateInstance<T>();

			foreach (var i in ReflectionHelper.GetPropertiesWithAttribute<PositionAttribute>(typeof(T))) {
				// if it has a position
				if (ReflectionHelper.TryGetAttributeOf<PositionAttribute>(i, out var posAttrib))

					// if there's an argument in the message for it
					if (posAttrib.Position >= 0 && msg.Args?.Length > posAttrib.Position &&
						i.PropertyType.IsAssignableFrom(ReflectionHelper.GetTypeOf(msg?.Args?[posAttrib.Position]))) {
						i.SetValue(res, msg.Args[posAttrib.Position]);
					} else {
						// if there's no optional attribute, or there's a required attribute
						if (!ReflectionHelper.TryGetAttributeOf<OptionalAttribute>(i, out var _) ||
							ReflectionHelper.TryGetAttributeOf<RequiredAttribute>(i, out var __)) {
							// yell at 'em
							return OneLinerFail($"Unable to set {i.Name}", out failErrMsg);
						}
					}
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