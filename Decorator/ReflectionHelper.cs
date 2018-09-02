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

		public static T EnsureAttributeGet<T, T2>()
				where T : Attribute {
			var attrib = GetAttributeOf<T>(typeof(T2));
			if (attrib == default) throw new CustomAttributeFormatException($"No attribute ({typeof(T).FullName}) was found on the item ({typeof(T2).FullName}) despite it needing the attribute.");
			return attrib;
		}

		public static T EnsureAttributeGet<T>(Type t)
				where T : Attribute {
			var attrib = GetAttributeOf<T>(t);
			if (attrib == default) throw new CustomAttributeFormatException($"No attribute was found on the item despite it needing the attribute.");
			return attrib;
		}

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

		public static IEnumerable<T> GetAttributesOf<T>(PropertyInfo t)
			where T : Attribute {
			foreach (var i in GetAttributesOf(t))
				if (i is T attrib)
					yield return attrib;
		}

		public static IEnumerable<MethodInfo> GetMethodsWithAttribute<T>(Type t)
			where T : Attribute {
			foreach (var i in t.GetMethods())
				if (GetAttributesOf<T>(i).Count() > 0)
					yield return i;
		}

		public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>(Type t)
			where T : Attribute {
			foreach (var i in t.GetProperties())
				if (GetAttributesOf<T>(i).Count() > 0)
					yield return i;
		}

		public static Type GetTypeOf<T>(T item)
			where T : class
			=> item == default(T) ? typeof(T) : item.GetType();

		public static bool TryGetAttributeOf<T>(Type t, out T attrib)
			where T : Attribute {
			attrib = GetAttributeOf<T>(t);
			return attrib != default(T);
		}

		public static bool TryGetAttributeOf<T>(PropertyInfo t, out T attrib)
			where T : Attribute {
			attrib = GetAttributeOf<T>(t);
			return attrib != default(T);
		}

		private static IEnumerable<Attribute> GetFrom(object[] attribs) {
			foreach (var i in attribs)
				if (i is Attribute attrib)
					yield return attrib;
		}

		public static int GetLargestPositionAttribute<T>()
			where T : class
			=> GetLargestPositionAttribute(typeof(T));

		public static int GetLargestPositionAttribute(Type t) {
			var largest = int.MinValue;

			foreach (var k in t.GetProperties())
				if (TryGetAttributeOf<PositionAttribute>(k, out var posAttrib) &&
					posAttrib.Position > largest)
						largest = posAttrib.Position;

			return largest;
		}
	}
}