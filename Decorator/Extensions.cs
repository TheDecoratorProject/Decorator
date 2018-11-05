using Decorator.ModuleAPI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator
{
	internal static class Extensions
	{
		public static int EstimateSize<T>(this BaseDecoratorModule[] decoratorInfos, T item)
		{
			var estimateSize = 0;

			for (var memberIndex = 0; memberIndex < decoratorInfos.Length; memberIndex++)
			{
				decoratorInfos[memberIndex].EstimateSize(item, ref estimateSize);
			}

			return estimateSize;
		}

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