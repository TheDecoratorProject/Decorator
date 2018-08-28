using Decorator.Attributes;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Decorator {

	public static class Deserializer {

		public static void DeserializeToEvent<T>(T eventClass, Message msg)
			where T : class {
			// if eventClass is null, it's a probably a static method.

			var matches = new List<(IMatchClass, Type, MethodInfo)>(8);

			foreach (var i in typeof(T).GetMethods()) {
				// make sure that if it's a static method, we're being passed null
				// or, make sure that if it's not a static method, we're being passed a value
				if ((i.IsStatic && eventClass == default(T)) ||
					!i.IsStatic && eventClass != default(T)) {
					var dhAttrib = (DeserializedHandlerAttribute)i.GetCustomAttribute(typeof(DeserializedHandlerAttribute), true);

					if (dhAttrib != default(DeserializedHandlerAttribute)) {
						var args = i.GetParameters();
						if (args.Length != 1) throw new CustomAttributeFormatException($"Any method with the {nameof(DeserializedHandlerAttribute)} Attribute must have one parameter.");

						var type = args[0].ParameterType;

						if (TryDeserializeGenerically(msg, type, out var genRes)) {
							var res = (IMatchClass)genRes;

							matches.Add((res, type, i));
						}
					}
				}
			}

			if (matches.Count > 0) {
				//TODO: don't use a tuple
				(IMatchClass, Type, MethodInfo) winner = default;
				bool duplicate = false;
				foreach (var i in matches) {
					if (winner == default) {
						duplicate = false;
						winner = i;
					} else if (i.Item1.MatchAmount > winner.Item1.MatchAmount) {
						duplicate = false;
						winner = i;
					} else if (i.Item1.MatchAmount == winner.Item1.MatchAmount) {
						duplicate = true;
					}
				}

				if(duplicate)
					throw new AmbiguousMatchException($"More then one canidates were found during deserialization.");

				winner.Item3.Invoke(eventClass, new object[] { Convert.ChangeType(winner.Item1.InnerClass, winner.Item2) });
				return;
			}
		}

		private static bool TryDeserializeGenerically(Message msg, Type msgType, out object result) {
			var args = new object[] { msg, null };

			var generic = typeof(Deserializer).GetMethod(nameof(InternalTryDeserialize), BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(msgType);
			var gmo = (bool)generic.Invoke(null, args);

			result = args[1];
			return gmo;
		}

		public static bool TryDeserialize<T>(Message msg, out T result) {
			var res = InternalTryDeserialize<T>(msg, out var test);
			result = test.InnerClass;
			return res;
		}

		private static bool InternalTryDeserialize<T>(Message msg, out MatchClass<T> result) {
			try {
				result = InternalDeserialize<T>(msg);
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
			return InternalDeserialize<T>(msg).InnerClass;
		}

		private static MatchClass<T> InternalDeserialize<T>(Message msg) {
			var t = typeof(T);
			var item = (T)Activator.CreateInstance(t);

			if (msg == null) throw new ArgumentNullException("Message is null.");

			var msgAttrib = (MessageAttribute)t.GetCustomAttribute(typeof(MessageAttribute), true);
			if (msgAttrib == default(MessageAttribute)) throw new CustomAttributeFormatException($"The type {t} doesn't have a [{nameof(MessageAttribute)}] attribute modifier defined on it.");
			if (msgAttrib.Type != msg.Type) throw new ArgumentException($"The message types don't match.");

			var matchAmount = 0;

			foreach (var i in t.GetProperties()) {
				var attribs = i.GetCustomAttributes();

				var posAttrib = (PositionAttribute)i.GetCustomAttribute(typeof(PositionAttribute), true);

				if (posAttrib != default(PositionAttribute)) {
					var setPropValue = true;

					foreach (var k in attribs) {
						if (k.GetType().GetInterface(nameof(Attributes.IPropertyAttributeBase)) != default)
							setPropValue = ((IPropertyAttributeBase)k).CheckRequirements<T>(i, msg, item, posAttrib) && setPropValue;
					}

					if (setPropValue) {
						matchAmount++;
						i.SetValue(item, msg.Args[posAttrib.Position]);
					}
				}

				/*
				var posAttrib = (PositionAttribute)i.GetCustomAttribute(typeof(PositionAttribute), true);

				if (posAttrib != default(PositionAttribute)) {
					if (msg.Args == null) throw new NullReferenceException($"Detected a position attribute, but the message arguments are null.");
					if (msg.Args.Length <= posAttrib.Position) throw new MessageException($"There aren't enough args in the message to match the position attribute.");
					if (i.PropertyType != msg.Args[posAttrib.Position].GetType()) throw new MessageException($"The message type in the args isn't equal to the property's value.");

					attribsSet++;
					i.SetValue(item, msg.Args[posAttrib.Position]);
				}*/
			}

			return new MatchClass<T>(item, matchAmount);
		}
	}

	internal interface IMatchClass {
		object InnerClass { get; }
		int MatchAmount { get; }
	}

	internal class MatchClass<T> : IMatchClass {

		public MatchClass(T @class, int matchAmount) {
			this.InnerClass = @class;
			this.MatchAmount = matchAmount;
		}

		public T InnerClass { get; }
		public int MatchAmount { get; set; }

		object IMatchClass.InnerClass => this.InnerClass;
		int IMatchClass.MatchAmount => this.MatchAmount;
	}
}