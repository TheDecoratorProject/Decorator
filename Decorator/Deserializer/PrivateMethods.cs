using Decorator.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator {
	// TODO: use enums instead of out strings

	public static partial class Deserializer {

		public static bool TryDeserializeToIEnumerable<T>(Message msg, out IEnumerable<T> itms, out string failErrMsg)
			where T : class, new() {
			itms = default;
			failErrMsg = default;

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

		//TODO: split up into smaller pieces
		
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

		/// <summary>
		/// Change an IEnumerable of objects to an IEnumerable of the specified type, using reflection to invoke a generic method.
		/// </summary>
		/// <param name="itms">The IEnumerable of objects</param>
		/// <param name="conv">The Type to convert it to</param>
		private static object GenericChangeEnumerable(IEnumerable<object> itms, Type conv) {
			return typeof(Deserializer)
					.GetMethod(nameof(ChangeEnumerable), BindingFlags.NonPublic | BindingFlags.Static)
					.MakeGenericMethod(conv)
					.Invoke(null, new object[] { itms });
		}

		/// <summary>
		/// Compile-time-safe generic method to change an IEnumerable of objects into an IEnumerable of the desired type.
		/// </summary>
		/// <typeparam name="T">The type</typeparam>
		/// <param name="ine">the input enumerable</param>
		private static IEnumerable<T> ChangeEnumerable<T>(IEnumerable<object> ine)
			=> ine.Select(x => (T)Convert.ChangeType(x, typeof(T)));

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