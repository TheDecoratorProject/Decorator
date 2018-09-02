using Decorator.Attributes;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Decorator {

	public static partial class Serializer {

		/// <summary>
		/// Serialize a single T item to a message
		/// </summary>
		/// <typeparam name="T">The type of item to serialize</typeparam>
		/// <param name="item">The item</param>
		/// <returns>A message with the item serialized</returns>
		public static Message Serialize<T>(T item) {
			Type t;

			t = EqualityComparer<T>.Default.Equals(item, default) ?
				typeof(T) :
				item.GetType();

			var msgAttrib = ReflectionHelper.EnsureAttributeGet<MessageAttribute, T>();

			var items = new Dictionary<int, object>();

			foreach (var i in t.GetProperties()) {
				var posAttrib = (PositionAttribute)i.GetCustomAttribute(typeof(PositionAttribute), true);

				if (posAttrib != default(PositionAttribute)) {
					items[posAttrib.Position] = i.GetValue(item);
				}
			}

			var itms = new object[items.Count];

			foreach (var i in items)
				if (i.Key >= itms.Length)
					throw new CustomAttributeFormatException($"There must be some skipped spaces in the PositionAttribute - check that your attributes go from 0 to x without skipping a number.");
				else itms[i.Key] = i.Value;

			return new Message(msgAttrib.Type, itms);
		}
	}
}