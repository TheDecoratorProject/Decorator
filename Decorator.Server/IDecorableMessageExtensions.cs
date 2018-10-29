using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decorator.Server
{
	public static class IDecorableMessageExtensions
	{
		private static ConcurrentDictionary<Type, string> _typeCache = new ConcurrentDictionary<Type, string>();

		public static string GetMessageType(this IDecorableMessage decorableMessage)
		{
			var type = decorableMessage.GetType();
			if (_typeCache.TryGetValue(type, out var msgType)) return msgType;

			var msgAttrib = type.GetCustomAttributes(true)
							.OfType<MessageAttribute>();

			if (msgAttrib.Count() == 0) throw new Exception();

			msgType = msgAttrib.First().Type;

			_typeCache.TryAdd(type, msgType);

			return msgType;
		}
	}
}
