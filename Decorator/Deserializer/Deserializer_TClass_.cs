using System.Linq;
using System.Reflection;

namespace Decorator
{
	public static class MethodDeserializer<TClass>
		where TClass : class
	{
		private static readonly FunctionWrapper _objToArray = new FunctionWrapper(
				typeof(MethodDeserializer<TClass>)
					.GetMethod(nameof(FromObjToArray), BindingFlags.Static | BindingFlags.NonPublic)
			);

		/// <summary>
		/// Invokes any methods with the <seealso cref="Attributes.DeserializedHandlerAttribute"/> attribute if the <paramref name="msg"/> can be deserialized to the first parameter in the method.
		/// </summary>
		/// <param name="instance">The instance of the <typeparamref name="TClass"/> to deserialize it to (use null for static)</param>
		/// <param name="msg">The message</param>
		public static void InvokeMethodFromMessage(TClass instance, BaseMessage msg)
		{
			var instanceObj = (object)instance;

			foreach (var i in MethodInvoker<TClass>.Cache)
			{
				if (i.Key.IsArray)
				{
					var arrayType = i.Key.GetElementType();
					if (Deserializer.TryDeserializeItems(arrayType, msg, out var enumerable))
					{
						var result = _objToArray.GetMethodFor(arrayType)(null, new object[] { enumerable });

						InvokeMethods(instanceObj, result, i.Value);
					}
				}
				else if (Deserializer.TryDeserializeItem(i.Key, msg, out var itm))
				{
					InvokeMethods(instanceObj, itm, i.Value);
				}
			}
		}

		private static void InvokeMethods(object instance, object result, MethodInfo[] methods)
		{
			foreach (var method in methods)
				MethodInvoker<TClass>.InvokeMethod(method, instance, result);
		}

		private static T[] FromObjToArray<T>(object[] objs)
			=> FromObj<T>(objs).ToArray();

		private static T[] FromObj<T>(object[] objs)
		{
			var res = new T[objs.Length];

			for (var i = 0; i < objs.Length; i++)
				res[i] = (T)objs[i];

			return res;
		}
	}

	/// <summary>Deserializes any message to a method in the TClass</summary>
	/// <typeparam name="TClass">The type of the class.</typeparam>
	public static class MethodDeserializer<TItem, TClass>
		where TClass : class
	{
		private static MethodInfo[] _methods;

		static MethodDeserializer()
		{
			_methods = MethodInvoker<TClass>.GetMethodsFor<TItem>();
		}

		/// <summary>
		/// Invokes any methods in the <see cref="TClass"/> <paramref name="instance"/> that have the <seealso cref="Attributes.DeserializedHandlerAttribute"/> attribute and accept the <typeparamref name="TItem"/> parameter as input.
		/// </summary>
		/// <typeparam name="TItem">The item to use</typeparam>
		/// <param name="instance">The instance of the <typeparamref name="TClass"/> to deserialize it to (use null for static)</param>
		/// <param name="item">The item to use to invoke stuff</param>
		public static void InvokeMethodFromItem(TClass instance, TItem item)
		{
			foreach (var i in _methods)
			{
				MethodInvoker<TClass>.InvokeMethod(i, instance, item);
			}
		}
	}
}