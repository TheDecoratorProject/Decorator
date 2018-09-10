using Decorator.Attributes;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Decorator {

	internal static class ReflectionHelper {

		private static readonly Cache<Type, object> _lambdaCache = new Cache<Type, object>();

		private static readonly Cache<Type, Func<object, object[], object>> _createCache = new Cache<Type, Func<object, object[], object>>();

		private static readonly MethodInfo _create = typeof(ReflectionHelper)
										.GetMethod(nameof(CreateGen));

		public static dynamic Create(Type t)
			=> _createCache.Retrieve(t, () =>
				IL.Wrap(
					_create
						.MakeGenericMethod(t)
					))
				(null, new object[] { });

		public static Func<T> CreateGen<T>()
			=> (Func<T>)_lambdaCache.Retrieve(typeof(T), () => Expression.Lambda<Func<T>>(Expression.New(typeof(T).GetConstructor(Type.EmptyTypes))).Compile());

		public static uint GetPosition(this PropertyInfo prop)
			=> prop.HasAttribute<PositionAttribute>(out var attrib) ?
				attrib.Position
				: default;

		public static PropertyInfo[] GetPositions(this Type t) {
			var props = t.GetProperties();
			var pos = new List<PropertyInfo>(props.Length);

			foreach (var i in props)
				if (i.HasAttribute<PositionAttribute>(out var _))
					pos.Add(i);

			return pos.ToArray();
		}

		public static string GetMessageType(this MemberInfo t)
			=> t.HasAttribute<MessageAttribute>(out var msgAttrib) ?
				msgAttrib.Type
				: default;

		public static bool HasAttribute<T>(this MemberInfo t, out T attrib)
																	where T : Attribute {
			var attribs = t.GetCustomAttributes(typeof(T), true);

			attrib = attribs.Length > 0 ?
				(T)attribs[0]
				: default;

			return attribs.Length > 0;
		}

		public static T[] GetAttributesOf<T>(this MemberInfo t)
					where T : Attribute
			=> (T[])t.GetCustomAttributes(typeof(T), true);
	}
}