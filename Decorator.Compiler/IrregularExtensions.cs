using Decorator.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator.Compiler
{
	internal static class IrregularExtensions
	{
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>
			(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			var seenKeys = new HashSet<TKey>();
			foreach (var element in source)
			{
				if (seenKeys.Add(keySelector(element)))
				{
					yield return element;
				}
			}

			yield break;
		}

		public static IEnumerable<MemberInfo> GetMembersRecursively(this Type type, BindingFlags bindingFlags)
		{
			var props = type
							.GetProperties(bindingFlags)
							.Cast<MemberInfo>();

			var fields = type
							.GetFields(bindingFlags)
							.Cast<MemberInfo>();

			var members = props.Concat(fields);

			if (type.BaseType != default)
			{
				var baseMembers = type.BaseType.GetMembersRecursively(bindingFlags);

				members = members
							.Concat(baseMembers)
							.DistinctBy((m) => m.Name);
			}

			return members
					.Where(x => x.GetCustomAttributes(true)
									.OfType<PositionAttribute>()
									.Count() > 0);
		}
	}
}