using Decorator.Attributes;

using System.Collections.Generic;

namespace Decorator {

	public static partial class Serializer {

		/// <summary>
		/// Serialize an IEnumerable of items to a message. Requires T to have a RepeatableAttribute.
		/// </summary>
		/// <typeparam name="T">The type of message to serialize</typeparam>
		/// <param name="items">The items</param>
		/// <returns>A Message that represents the serialization of the message.</returns>
		public static Message SerializeEnumerable<T>(IEnumerable<T> items) {
			var msgAttrib = ReflectionHelper.EnsureAttributeGet<MessageAttribute, T>();
			ReflectionHelper.EnsureAttributeGet<RepeatableAttribute, T>();

			var args = new List<object>();

			foreach (var i in items) {
				var k = Serialize(i);

				args.AddRange(k.Args);
			}

			return new Message(msgAttrib.Type, args.ToArray());
		}
	}
}