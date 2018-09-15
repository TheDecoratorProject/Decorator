using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Decorator {

	public static class AttributeCache<T>
		where T : Attribute {
		private static ConcurrentDictionary<MemberInfo, T[]> _memberInfoCache { get; } = new ConcurrentDictionary<MemberInfo, T[]>();

		public static bool TryHasAttribute(MemberInfo member, out T[] result) {
			if (!_memberInfoCache.TryGetValue(member, out result)) {
				result = member.GetCustomAttributes<T>().ToArray();

				if (result.Length == 0) {
					_memberInfoCache.TryAdd(member, result);
					return false;
				}

				_memberInfoCache.TryAdd(member, result);

				return true;
			}

			return result != default && result.Length != 0;
		}

		public static bool HasAttribute(MemberInfo member) {
			if (!_memberInfoCache.TryGetValue(member, out var result)) {
				result = member.GetCustomAttributes<T>().ToArray();

				if (result.Length == 0) {
					_memberInfoCache.TryAdd(member, result);
					return false;
				}

				_memberInfoCache.TryAdd(member, result);

				return true;
			}

			return result != default && result.Length != 0;
		}
	}
}