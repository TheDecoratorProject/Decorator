using Decorator.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator {
	internal static class ReflectionHelper {

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

		public static IEnumerable<Attribute> GetAttributesOf(Type t)
			=> GetFrom(t.GetCustomAttributes(true)).ToArray();

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
}