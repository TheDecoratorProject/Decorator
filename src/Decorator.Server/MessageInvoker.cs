using SwissILKnife;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Decorator.Server
{
	public static class MessageInvoker<T>
	{
		private static readonly ConcurrentDictionary<Type, Func<object, object[], object>> _handlers;

		static MessageInvoker()
		{
			_handlers = new ConcurrentDictionary<Type, Func<object, object[], object>>();

			foreach (var i in typeof(T).GetMethods()
										.Where(x => x.GetCustomAttributes(true).Count(y => y is MessageHandlerAttribute) > 0))
			{
				_handlers.TryAdd(i.GetParameters()[0].ParameterType, MethodWrapper.Wrap(i));
			}
		}

		public static object Invoke(T instance, BaseMessage m)
		{
			foreach (var i in _handlers)
			{
				var args = new object[] { m.Arguments, 0, null };

				var tyo = typeof(DConverter<>)
					.MakeGenericType(i.Key);
				var method = tyo
					.GetMethod("TryDeserialize", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, new Type[] { typeof(object[]), typeof(int).MakeByRefType(), i.Key.MakeByRefType() }, new ParameterModifier[] { });

				if ((bool)method
					.Invoke(instance, args))
				{
					//TODO: split up

					return i.Value(instance, new object[] { args[2] });
				}
			}

			return null;
		}
	}
}