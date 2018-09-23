using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator.Caching {
	public interface IHashcodeDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> {
		bool TryAdd(TKey key, TValue value);

		bool TryGetValue(TKey key, out TValue value);

		IEnumerable<KeyValuePair<TKey, TValue>> GetItems();
	}
}
