
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Decorator {
	public static class AttributeCache<T>
		where T : Attribute {
		private static ConcurrentDictionary<MemberInfo, T[]> _memberInfoCache { get; set; } = new ConcurrentDictionary<MemberInfo, T[]>();

		public static bool HasAttribute(MemberInfo member, out T[] result) {
			if(!_memberInfoCache.TryGetValue(member, out result)) {
				result = member.GetCustomAttributes<T>().ToArray();

				if (result.Length == 0) {
					_memberInfoCache.TryAdd(member, result);
					return false;
				}

				_memberInfoCache.TryAdd(member, result);

				return true;
			}

			if (result == default || result.Length == 0) return false;

			return true;
		}
	}
}