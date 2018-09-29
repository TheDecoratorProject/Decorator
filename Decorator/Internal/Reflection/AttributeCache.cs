using Decorator.Caching;

using System;
using System.Linq;
using System.Reflection;

namespace Decorator
{
	internal static class AttributeCache<T>
		where T : Attribute
	{
		private static readonly ConcurrentHashcodeDictionary<MemberInfo, T[]> _memberInfoCache = new ConcurrentHashcodeDictionary<MemberInfo, T[]>();

		public static bool TryHasAttribute(MemberInfo member, out T[] result)
		{
			if (!_memberInfoCache.TryGetValue(member, out result))
			{
				result = member.GetCustomAttributes<T>().ToArray();

				if (result.Length == 0)
				{
					_memberInfoCache.TryAdd(member, result);
					return false;
				}

				_memberInfoCache.TryAdd(member, result);

				return true;
			}

			return result != default && result.Length != 0;
		}

		public static bool HasAttribute(MemberInfo member)
			=> TryHasAttribute(member, out var _);
	}
}