using Decorator.Attributes;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

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
		
	}
}