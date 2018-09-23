using System.Collections.Generic;

namespace Decorator.Caching {

	internal interface IHashcodeDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> {

		bool TryAdd(TKey key, TValue value);

		bool TryGetValue(TKey key, out TValue value);

		IEnumerable<KeyValuePair<TKey, TValue>> GetItems();
	}
}