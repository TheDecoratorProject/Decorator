using Decorator.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Decorator {
	public static class Deserializer {
		public static void DeserializeToEvent<T>(T eventClass, Message msg) {
			//var msgAttrib = (MessageAttribute)typeof(T).GetCustomAttribute(typeof(MessageAttribute));
			//if (msgAttrib == default(MessageAttribute)) throw new CustomAttributeFormatException($"The type {typeof(T)} doesn't have a [{nameof(MessageAttribute)}] attribute modifier defined on it.");

			// if eventClass is null, it's a probably a static method.

			foreach (var i in typeof(T).GetMethods()) {
				var dhAttrib = (DeserializedHandlerAttribute)i.GetCustomAttribute(typeof(DeserializedHandlerAttribute), true);

				if (dhAttrib != default(DeserializedHandlerAttribute)) {
					var args = i.GetParameters();
					if (args.Length != 1) throw new CustomAttributeFormatException($"Any method with the {nameof(DeserializedHandlerAttribute)} Attribute must have one parameter.");

					var type = args[0].ParameterType;

					if (TryDeserializeGenerically(msg, type, out var res)) {
						i.Invoke(eventClass, new object[] { Convert.ChangeType(res, type) });
						return;
					}
				}
			}
		}

		private static bool TryDeserializeGenerically(Message msg, Type msgType, out object result) {
			var args = new object[] { msg, null };

			var generic = typeof(Deserializer).GetMethod(nameof(TryDeserialize), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(msgType);
			var gmo = (bool)generic.Invoke(null, args);

			result = args[1];
			return gmo;
		}

		public static bool TryDeserialize<T>(Message msg, out T result) {
			try {
				result = Deserialize<T>(msg);
				return true;
			} catch (Exception ex) when (
				ex is ArgumentNullException ||
				ex is CustomAttributeFormatException ||
				ex is NullReferenceException ||
				ex is MessageException) {
				result = default;
				return false;
			}
		}

		public static T Deserialize<T>(Message msg) {
			var t = typeof(T);
			var item = (T)Activator.CreateInstance(t);

			if (msg == null) throw new ArgumentNullException("Message is null.");

			var msgAttrib = (MessageAttribute)t.GetCustomAttribute(typeof(MessageAttribute), true);
			if (msgAttrib == default(MessageAttribute)) throw new CustomAttributeFormatException($"The type {t} doesn't have a [{nameof(MessageAttribute)}] attribute modifier defined on it.");
			if (msgAttrib.Type != msg.Type) throw new ArgumentException($"The message types don't match.");

			var attribsSet = 0;

			foreach (var i in t.GetProperties()) {
				var posAttrib = (PositionAttribute)i.GetCustomAttribute(typeof(PositionAttribute), true);

				if (posAttrib != default(PositionAttribute)) {
					if (msg.Args == null) throw new NullReferenceException($"Detected a position attribute, but the message arguments are null.");
					if (msg.Args.Length <= posAttrib.Position) throw new MessageException($"There aren't enough args in the message to match the position attribute.");
					if (i.PropertyType != msg.Args[posAttrib.Position].GetType()) throw new MessageException($"The message type in the args isn't equal to the property's value.");

					attribsSet++;
					i.SetValue(item, msg.Args[posAttrib.Position]);
				}
			}

			if (attribsSet != msg.Args.Length) throw new MessageException($"The class specified has less position attributes then the message has arguments.");

			return item;
		}
	}

	public class MessageException : Exception {
		public MessageException(string msg) : base(msg) {

		}
	}
}