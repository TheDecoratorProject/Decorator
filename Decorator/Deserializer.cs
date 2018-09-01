using Decorator.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator {

	internal interface IMatchClass {
		object InnerClass { get; }
		int MatchAmount { get; }
	}

	public static class Deserializer {

		public static bool DeserializeToEvent<T>(T eventClass, Message msg, params object[] extraParams)
			where T : class {
			var success = false;

			foreach (var i in ReflectionHelper.GetDeserializableHandlers(ReflectionHelper.GetTypeOf(eventClass))) {
				var args = i.GetParameters();

				if (args?.Length < 1) throw new CustomAttributeFormatException($"Invalid [{nameof(DeserializedHandlerAttribute)}] - must have at least one parameter");

				if (TryDeserializeGenerically(msg, args[0].ParameterType, out var param, out var _)) {
					success = true;

					var invokeArgs = new object[extraParams.Length + 1];
					invokeArgs[0] = param;
					for (int k = 0; k < extraParams.Length; k++)
						invokeArgs[k + 1] = extraParams[k];

					i.Invoke(eventClass, invokeArgs);
				}
			}

			return success;
		}

		public static bool TryDeserialize<T>(Message msg, out T res, out string failErrMsg)
			where T : class, new() {
			// ensure some basic things

			res = default;
			failErrMsg = default;

			ReflectionHelper.CheckNull(msg, nameof(msg));

			var msgAttrib = ReflectionHelper.EnsureAttributeGet<MessageAttribute, T>();
			if (msgAttrib.Type != msg.Type) return OneLinerFail("The base message types aren't equal.", out failErrMsg);

			// if there's a limit on the amount of arguments set for the item

			if (ReflectionHelper.TryGetAttributeOf<ArgumentLimitAttribute>(typeof(T), out var limit))
				if (msg?.Args?.Length > limit.ArgLimit)
					return OneLinerFail($"Surpassed the maximum amount of args bound by the {nameof(ArgumentLimitAttribute)}", out failErrMsg);

			// loop through every property

			res = Activator.CreateInstance<T>();

			foreach (var i in typeof(T).GetProperties())

				// if it has a position
				if (ReflectionHelper.TryGetAttributeOf<PositionAttribute>(i, out var posAttrib))

					// if there's an argument in the message for it
					if (posAttrib.Position >= 0 && msg.Args?.Length > posAttrib.Position &&
						i.PropertyType.IsAssignableFrom(ReflectionHelper.GetTypeOf(msg?.Args?[posAttrib.Position]))) {
						i.SetValue(res, msg.Args[posAttrib.Position]);
					} else {
						// if there's no optional attribute, or there's a required attribute
						if (!ReflectionHelper.TryGetAttributeOf<OptionalAttribute>(i, out var _) ||
							ReflectionHelper.TryGetAttributeOf<RequiredAttribute>(i, out var __))

							// yell at 'em
							return OneLinerFail($"Unable to set {i.Name}", out failErrMsg);
					}

			return true;
		}

		public static bool TryDeserializeGenerically(Message msg, Type desType, out object res, out string failErrMsg) {
			var args = new object[] { msg, null, null };

			var generic = typeof(Deserializer).GetMethod(nameof(TryDeserialize), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(desType);
			var gmo = (bool)generic.Invoke(null, args);

			res = args[1];
			failErrMsg = (string)args[2];

			return gmo;
		}

		private static bool OneLinerFail(string amt, out string failErrMsg) {
			failErrMsg = amt;
			return false;
		}
	}

	internal static class ReflectionHelper {
		//private static ConcurrentDictionary<Type, IEnumerable<Attribute>> AttributeCache = new ConcurrentDictionary<Type, IEnumerable<Attribute>>();
		//private static ConcurrentDictionary<Type, MethodInfo> DeserializeMethodHandlers = new ConcurrentDictionary<Type, MethodInfo>();

		public static void CheckNull<T>(T item, string paramName)
			where T : class {
			if (item == default(T)) throw new ArgumentNullException(paramName);
		}

		public static T EnsureAttributeGet<T, T2>(T2 itm)
			where T : Attribute
			where T2 : class
			=> GetAttributeOf<T>(GetTypeOf(itm));

		public static T EnsureAttributeGet<T, T2>()
			where T : Attribute
			where T2 : class
			=> GetAttributeOf<T>(typeof(T2));

		public static T GetAttributeOf<T>(Type t)
			where T : Attribute {
			foreach (var i in GetAttributesOf(t))
				if (i is T attrib)
					return attrib;
			return default;
		}

		public static T GetAttributeOf<T>(PropertyInfo t)
			where T : Attribute {
			foreach (var i in GetAttributesOf(t))
				if (i is T attrib)
					return attrib;
			return default;
		}

		public static IEnumerable<Attribute> GetAttributesOf(Type t) {
			//if (AttributeCache.TryGetValue(t, out var attribs)) return attribs;

			var attribs = GetFrom(t.GetCustomAttributes(true)).ToArray();
			//AttributeCache.TryAdd(t, attribs.ToArray());

			return attribs;
		}

		public static IEnumerable<Attribute> GetAttributesOf(PropertyInfo t)
			=> GetFrom(t.GetCustomAttributes(true));

		public static IEnumerable<Attribute> GetAttributesOf(MethodInfo t)
			=> GetFrom(t.GetCustomAttributes(true));

		public static IEnumerable<Attribute> GetAttributesOf<T>(MethodInfo t)
			where T : Attribute {
			foreach (var i in GetAttributesOf(t))
				if (i is T attrib)
					yield return attrib;
		}

		public static IEnumerable<T> GetAttributesOf<T>(Type t)
			where T : Attribute {
			foreach (var i in GetAttributesOf(t))
				if (i is T attrib)
					yield return attrib;
		}

		public static IEnumerable<MethodInfo> GetDeserializableHandlers(Type t) {
			foreach (var i in t.GetMethods())
				if (GetAttributesOf<DeserializedHandlerAttribute>(i).Count() > 0)
					yield return i;
		}

		public static Type GetTypeOf<T>(T item)
																			where T : class
			=> item == default(T) ? typeof(T) : item.GetType();

		public static bool TryGetAttributeOf<T>(Type t, out T attrib)
			where T : Attribute
			=> (attrib = GetAttributeOf<T>(t)) != default(T);

		public static bool TryGetAttributeOf<T>(PropertyInfo t, out T attrib)
			where T : Attribute
			=> (attrib = GetAttributeOf<T>(t)) != default(T);

		private static IEnumerable<Attribute> GetFrom(object[] attribs) {
			foreach (var i in attribs)
				if (i is Attribute attrib)
					yield return attrib;
		}
	}

	internal class MatchClass<T> : IMatchClass {

		public MatchClass(T @class, int matchAmount) {
			this.InnerClass = @class;
			this.MatchAmount = matchAmount;
		}

		public T InnerClass { get; }
		object IMatchClass.InnerClass => this.InnerClass;
		public int MatchAmount { get; set; }
		int IMatchClass.MatchAmount => this.MatchAmount;
	}
}