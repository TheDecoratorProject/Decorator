using Decorator.Attributes;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Decorator {

	public static class ReflectionHelper {

		public static bool HasAttribute<T>(this MemberInfo t, out T attrib)
			where T : Attribute {
			var attribs = t.GetCustomAttributes(typeof(T), true);

			attrib = attribs.Length > 0 ?
				(T)attribs[0]
				: default;

			return attribs.Length > 0;
		}

		public static T[] GetAttributesOf<T>(this MemberInfo t)
			where T : Attribute {
			return (T[])t.GetCustomAttributes(typeof(T), true);
		}

		public static string GetMessageType(this MemberInfo t) {
			if (t.HasAttribute<MessageAttribute>(out var msgAttrib))
				return msgAttrib.Type;
			else return default;
		}

		public static PropertyInfo[] GetPositions(this Type t) {
			var pos = new List<PropertyInfo>();

			foreach (var i in t.GetProperties())
				if (i.HasAttribute<PositionAttribute>(out var _))
					pos.Add(i);

			return pos.ToArray();
		}

		public static uint GetPosition(this PropertyInfo prop) {
			if (prop.HasAttribute<PositionAttribute>(out var attrib))
				return attrib.Position;
			else return default;
		}
	}
}