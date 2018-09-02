using Decorator.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator {
	// TODO: use enums instead of out strings

	public static partial class Deserializer {

		//TODO: split up into smaller pieces
		
		private static bool DeserializeMessageToIEnumerableAndInvoke<T>(T eventClass, Type desType, Message msg, MethodInfo method, object[] extraParams) {
			//TODO: check if it is an IEnumerable<> itself and not if it inherits IEnumerable

			if (desType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable))) {
				var genArg = desType.GenericTypeArguments[0];
				var genMsgAttrib = ReflectionHelper.EnsureAttributeGet<MessageAttribute>(genArg);

				if (ReflectionHelper.TryGetAttributeOf<RepeatableAttribute>(genArg, out var _) &&
					genMsgAttrib.Type == msg.Type) {
					// get the largest position of the attribute
					//TODO: check int.MinValue
					var largest = ReflectionHelper.GetLargestPositionAttribute(genArg) + 1;

					// ensure that the repeat amount is fine
					if (msg.Args.Length % largest != 0) return false;

					// List< desType >, just objects for reflection purposes
					var itms = new List<object>();

					for (var k = 0; k < msg.Args.Length / largest; k++) {

						// take a specific chunk of the Message into a smaller sized Message to deserialize
						var msgItms = new List<object>();

						for (var j = 0; j < largest; j++) {
							msgItms.Add(msg.Args[(k * largest) + j]);
						}

						// create said above comment
						var resMsg = new Message(genMsgAttrib.Type, msgItms.ToArray());
						
						// des.
						if (TryDeserializeGenerically(resMsg, genArg, out var result, out var __)) itms.Add(result);
						else return false;
					}

					// change List<object> to IEnumerable< desType >
					var resItms = GenericChangeEnumerable(itms, genArg);

					// invoke
					var invokeArgs = new object[extraParams.Length + 1];
					invokeArgs[0] = resItms;
					for (var k = 0; k < extraParams.Length; k++)
						invokeArgs[k + 1] = extraParams[k];

					method.Invoke(eventClass, invokeArgs);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Change an IEnumerable of objects to an IEnumerable of the specified type, using reflection to invoke a generic method.
		/// </summary>
		/// <param name="itms">The IEnumerable of objects</param>
		/// <param name="conv">The Type to convert it to</param>
		private static object GenericChangeEnumerable(IEnumerable<object> itms, Type conv) {
			var generic = typeof(Deserializer).GetMethod(nameof(ChangeEnumerable), BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(conv);
			var gmo = generic.Invoke(null, new object[] { itms });

			return gmo;
		}

		/// <summary>
		/// Compile-time-safe generic method to change an IEnumerable of objects into an IEnumerable of the desired type.
		/// </summary>
		/// <typeparam name="T">The type</typeparam>
		/// <param name="ine">the input enumerable</param>
		private static IEnumerable<T> ChangeEnumerable<T>(IEnumerable<object> ine) {
			var objs = new List<T>();

			foreach (var i in ine) {
				objs.Add((T)Convert.ChangeType(i, typeof(T)));
			}

			return objs;
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