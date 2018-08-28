using Decorator.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Decorator
{
	public static class Serializer {
		public static Message Serialize<T>(T item) {
			var msgAttrib = (MessageAttribute)typeof(T).GetCustomAttribute(typeof(MessageAttribute));
			if (msgAttrib == default(MessageAttribute)) throw new CustomAttributeFormatException($"The type {typeof(T)} doesn't have a [{nameof(MessageAttribute)}] attribute modifier defined on it.");

			var items = new Dictionary<int, object>();

			foreach(var i in typeof(T).GetProperties()) {
				var posAttrib = (PositionAttribute)i.GetCustomAttribute(typeof(PositionAttribute), true);

				if (posAttrib != default(PositionAttribute)) {
					items[posAttrib.Position] = i.GetValue(item);
				}
			}

			object[] itms = new object[items.Count];

			foreach (var i in items)
				if (i.Key >= itms.Length)
					throw new CustomAttributeFormatException($"There must be some skipped spaces in the PositionAttribute - check that your attributes go from 0 to x without skipping a number.");
				else itms[i.Key] = i.Value;

			return new Message(msgAttrib.Type, itms);
		}
	}
}