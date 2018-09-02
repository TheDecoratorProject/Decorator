using Decorator.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator {
	// TODO: use enums instead of out strings

	public static class Deserializer {

		/// <summary>Deserialize a Message, and find the appropiate DeserializedHandlerAttribute that takes the type of the deserialized message.</summary>
		/// <typeparam name="T">The class to look for methods in</typeparam>
		/// <param name="eventClass">An instance of the class. Leave it to null to find static methods</param>
		/// <param name="msg">The message to be deserializing</param>
		/// <param name="extraParams">Additional parameters that you wish to pass into the function</param>
		/// <returns>If it succeeded or not.</returns>
		/// <exception cref="CustomAttributeFormatException">DeserializedHandlerAttribute</exception>
		public static bool TryDeserializeToEvent<T>(T eventClass, Message msg, params object[] extraParams)
			where T : class, new() {
			var success = false;

			ReflectionHelper.CheckNull(msg, nameof(msg));

			// loop through every deserializable handler
			foreach (var i in ReflectionHelper.GetMethodsWithAttribute<DeserializedHandlerAttribute>(ReflectionHelper.GetTypeOf(eventClass))) {
				// get the args
				var args = i.GetParameters();

				if (args?.Length < 1) throw new CustomAttributeFormatException($"Invalid [{nameof(DeserializedHandlerAttribute)}] - must have at least one parameter");

				// get the type
				var desType = args[0].ParameterType;

				// try to convert to an IEnumerable
				var isEnumerable = InvokeIEnumerableMethod(eventClass, desType, msg, i, extraParams, out var _);

				// if we didn't convert it to an ienumerable and we can des. to a desType
				if (!isEnumerable &&
					!desType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)) &&
					TryDeserializeGenerically(msg, desType, out var param, out var _)) {
					success = true;

					InvokeMethod(eventClass, i, param, extraParams);
				}
			}

			return success;
		}

		/// <summary>Attempts to deserialize the Message to an IEnumerable of type T</summary>
		/// <typeparam name="T">The type of message that it needs to deserialize</typeparam>
		/// <param name="msg">The message to deserialize</param>
		/// <param name="itms">The end result IEnumerable of Ts</param>
		/// <param name="failErrMsg">In event of failure, the error message</param>
		/// <returns>If it succeeded</returns>
		public static bool TryDeserializeToIEnumerable<T>(Message msg, out IEnumerable<T> itms, out string failErrMsg)
			where T : class, new() {
			itms = default;
			failErrMsg = default;

			ReflectionHelper.CheckNull(msg, nameof(msg));

			// make sure it supports IEnumerable deserialization
			ReflectionHelper.EnsureAttributeGet<RepeatableAttribute>(typeof(T));
			ReflectionHelper.EnsureAttributeGet<MessageAttribute>(typeof(T));

			var msgPosLength = ReflectionHelper.GetLargestPositionAttribute(typeof(T))
							 + 1;

			// ensure that the message length is just a bunch of the same message
			if (msg.Args.Length % msgPosLength != 0) return OneLinerFail("Uneven amount of message params", out failErrMsg);

			var innerMessages = msg.Args.Length / msgPosLength;

			var resItms = new List<T>();

			// for every "inner message"
			for (var i = 0; i < innerMessages; i++) {
				// copy it and try to deserialize it
				var args = new object[msgPosLength];
				Array.Copy(msg.Args, i * msgPosLength, args, 0, msgPosLength);

				if (!TryDeserialize<T>(new Message(msg.Type, args), out var res, out var failDes)) return OneLinerFail($"Unable to deserialize the ({i})th message: {failDes}", out failErrMsg);

				resItms.Add(res);
			}

			itms = resItms;
			return true;
		}

		/// <summary>
		/// Attempts to deserialize a message to a class.
		/// </summary>
		/// <typeparam name="T">The class to be deserializing the Message to</typeparam>
		/// <param name="msg">The Message needing deserialization</param>
		/// <param name="res">The result, T</param>
		/// <param name="failErrMsg">A reason for failure.</param>
		/// <returns>If it succeeded.</returns>
		public static bool TryDeserialize<T>(Message msg, out T res, out string failErrMsg)
			where T : class, new() {
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
		/// <summary>
		/// Invokes the TryDeserilize method, accepting Types instead of a compile-time T constraint.
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="desType">The type to deserialize to</param>
		/// <param name="res">The result</param>
		/// <param name="failErrMsg">The reason for failure</param>
		/// <returns>If it succeded.</returns>
		private static bool TryDeserializeGenerically(Message msg, Type desType, out object res, out string failErrMsg) {
			var args = new object[] { msg, null, null };

			var gmo = (bool)typeof(Deserializer)
							.GetMethod(nameof(TryDeserialize), BindingFlags.Public | BindingFlags.Static)
							.MakeGenericMethod(desType)
							.Invoke(null, args);

			res = args[1];
			failErrMsg = (string)args[2];

			return gmo;
		}

		private static bool InvokeIEnumerableMethod<T>(T eventClass, Type desType, Message msg, MethodInfo meth, object[] extraParams, out string failErrMsg)
							where T : class, new() {
			failErrMsg = default;
			//TODO: check if it is an IEnumerable<> itself and not if it inherits IEnumerable

			if (desType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable))) {
				var genArg = desType.GenericTypeArguments[0];

				var args = new object[] { msg, null, null };

				if ((bool)typeof(Deserializer)
						.GetMethod(nameof(TryDeserializeToIEnumerable), BindingFlags.Public | BindingFlags.Static)
						.MakeGenericMethod(genArg)
						.Invoke(null, args)) {
					InvokeMethod<T>(eventClass, meth, args[1], extraParams);
					return true;
				}

				failErrMsg = (string)args[2];
			}

			return false;
		}

		private static void InvokeMethod<T>(T eventClass, MethodInfo meth, object invokItm, object[] extraParams)
					where T : class {
			var invokeArgs = new object[extraParams.Length + 1];

			Array.Copy(extraParams, 0, invokeArgs, 1, extraParams.Length);

			invokeArgs[0] = invokItm;

			meth.Invoke(eventClass, invokeArgs);
		}

		/// <summary>
		/// Small helper method to prevent repeating code.
		/// </summary>
		/// <param name="amt">the msg</param>
		/// <param name="failErrMsg">the out failErrMsg</param>
		private static bool OneLinerFail(string amt, out string failErrMsg) {
			failErrMsg = amt;
			return false;
		}
	}
}