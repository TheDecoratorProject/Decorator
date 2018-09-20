using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Decorator {
	/// <summary>Deserializes any message to a method in the TClass</summary>
	/// <typeparam name="TClass">The type of the class.</typeparam>
	public static class Deserializer<TClass>
		where TClass : class {

		static Deserializer()
			=> MethodDeserializerManager = new MethodDeserializerManager<TClass>();

		private static readonly FunctionWrapper _objToArray = new FunctionWrapper(
				typeof(Deserializer<TClass>)
					.GetMethod(nameof(FromObjToArray), BindingFlags.Static | BindingFlags.NonPublic)
			);

		public static MethodDeserializerManager<TClass> MethodDeserializerManager { get; }

		/// <summary>
		/// Invokes any methods in the <see cref="TClass"/> <paramref name="instance"/> that have the <seealso cref="Attributes.DeserializedHandlerAttribute"/> attribute and accept the <typeparamref name="TItem"/> parameter as input.
		/// </summary>
		/// <typeparam name="TItem">The item to use</typeparam>
		/// <param name="instance">The instance of the <typeparamref name="TClass"/> to deserialize it to (use null for static)</param>
		/// <param name="item">The item to use to invoke stuff</param>
		public static void DeserializeItemToMethod<TItem>(TClass instance, TItem item) {
			foreach (var i in MethodDeserializerManager.GetMethodsFor<TItem>()) {
				MethodDeserializerManager.InvokeMethod<TItem>(i, instance, item);
			}
		}

		/// <summary>
		/// Invokes any methods with the <seealso cref="Attributes.DeserializedHandlerAttribute"/> attribute if the <paramref name="msg"/> can be deserialized to the first parameter in the method.
		/// </summary>
		/// <param name="instance">The instance of the <typeparamref name="TClass"/> to deserialize it to (use null for static)</param>
		/// <param name="msg">The message</param>
		public static void DeserializeMessageToMethod(TClass instance, BaseMessage msg) {
			var instanceObj = (object)instance;

			foreach (var i in MethodDeserializerManager.Cache) {
				if (i.Key.GenericTypeArguments.Length > 0) {
					var genArg = i.Key.GenericTypeArguments[0];

					if (typeof(IEnumerable).IsAssignableFrom(i.Key) &&
						Deserializer.TryDeserializeItems(genArg, msg, out var enumerable)) {
						var result = _objToArray.GetMethodFor(genArg)(null, new object[] { enumerable });

						foreach (var k in i.Value)
							MethodDeserializerManager.InvokeMethod(k, instanceObj, result);
					}
				} else if (Deserializer.TryDeserializeItem(i.Key, msg, out var itm)) {
					foreach (var k in i.Value)
						MethodDeserializerManager.InvokeMethod(k, instanceObj, itm);
				}
			}
		}

		private static IEnumerable<T> FromObjToArray<T>(IEnumerable<object> objs)
			=> FromObj<T>(objs).ToArray();

		private static IEnumerable<T> FromObj<T>(IEnumerable<object> objs) {
			foreach (var i in objs)
				yield return (T)i;
		}
	}
}
