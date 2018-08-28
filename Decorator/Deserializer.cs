using Decorator.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Decorator {
	public static class Deserializer {
		public static T Deserialize<T>(Message msg) {
			var t = typeof(T);
			var item = (T)Activator.CreateInstance(t);

			if (msg == null) throw new ArgumentNullException("Message is null.");

			var msgAttrib = (MessageAttribute)t.GetCustomAttribute(typeof(MessageAttribute), true);
			if (msgAttrib == default(MessageAttribute)) throw new CustomAttributeFormatException($"The type {t} doesn't have a [{nameof(MessageAttribute)}] attribute modifier defined on it.");
			if (msgAttrib.Type != msg.Type) throw new ArgumentNullException($"The message types don't match.");

			foreach (var i in t.GetProperties()) {
				var posAttrib = (PositionAttribute)i.GetCustomAttribute(typeof(PositionAttribute), true);

				if (posAttrib != default(PositionAttribute)) {
					if (msg.Args == null) throw new NullReferenceException($"Detected a position attribute, but the message arguments are null.");
					if (msg.Args.Length <= posAttrib.Position) throw new Exception($"There aren't enough args in the message to match the position attribute.");
					if (i.PropertyType != msg.Args[posAttrib.Position].GetType()) throw new Exception($"The message type in the args isn't equal to the property's value.");

					i.SetValue(item, msg.Args[posAttrib.Position]);
				}
			}

			return item;
		}
	}
}