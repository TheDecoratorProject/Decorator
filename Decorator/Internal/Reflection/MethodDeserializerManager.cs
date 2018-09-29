using Decorator.Attributes;
using Decorator.Caching;
using Decorator.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator
{
	internal static class MethodInvoker<TClass>
		where TClass : class
	{
		private static readonly Type _tclassType = typeof(TClass);

		static MethodInvoker()
		{
			Cache = new HashcodeDictionary<Type, MethodInfo[]>();

			MethodInfoCache = new HashcodeDictionary<MethodInfo, Action<object, object>>();

			//TODO: clean
			// with like a reflection helper class
			// or something

			var dict = new Dictionary<Type, List<MethodInfo>>();

			foreach (var i in _tclassType.GetMethods().Where(x => AttributeCache<DeserializedHandlerAttribute>.HasAttribute(x)))
				if (dict.TryGetValue(i.GetParameters()[0].ParameterType, out var val))
					val.Add(i);
				else dict[i.GetParameters()[0].ParameterType] = new List<MethodInfo> { i };

			foreach (var i in dict)
			{
				Cache.TryAdd(i.Key, i.Value.ToArray());
			}
		}

		public static HashcodeDictionary<Type, MethodInfo[]> Cache;

		public static HashcodeDictionary<MethodInfo, Action<object, object>> MethodInfoCache;

		public static MethodInfo[] GetMethodsFor<TItem>()
			=> GetMethodsFor(typeof(TItem));

		public static MethodInfo[] GetMethodsFor(Type item)
		{
			if (Cache.TryGetValue(item, out var res)) return res;
			throw new LackingMethodsException(item);
		}

		public static void InvokeMethod<TItem>(MethodInfo method, TClass instance, TItem item)
			=> InvokeMethod(method, (object)instance, item);

		public static void InvokeMethod<TItem>(MethodInfo method, object instance, TItem item)
		{
			if (!MethodInfoCache.TryGetValue(method, out var invoke))
			{
				invoke = method
							.GetSingleInvokable();

				MethodInfoCache.TryAdd(method, invoke);
			}

			invoke(instance, item);
		}
	}
}