using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Decorator.Caching
{
	internal class ConcurrentHashcodeDictionary<TKey, TValue> : IHashcodeDictionary<TKey, TValue>
	{
		public ConcurrentHashcodeDictionary()
		{
			Dictionary = new ConcurrentHashcodeDictionary<TValue>();
			DictionaryKeys = new ConcurrentHashcodeDictionary<TKey>();
		}

		public ConcurrentHashcodeDictionary<TValue> Dictionary { get; set; }
		public ConcurrentHashcodeDictionary<TKey> DictionaryKeys { get; set; }

		public bool TryAdd(TKey key, TValue value)
		{
			var hashcode = key.GetHashCode();

			return Dictionary.TryAdd(hashcode, value) &&
					DictionaryKeys.TryAdd(hashcode, key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return Dictionary.TryGetValue(key.GetHashCode(), out value);
		}

		public IEnumerable<KeyValuePair<TKey, TValue>> GetItems()
		{
			return Dictionary.Zip(DictionaryKeys, (a, b) => new KeyValuePair<TKey, TValue>(b.Value, a.Value));
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new HashcodeDictionaryEnumerator<TKey, TValue>(this);

		IEnumerator IEnumerable.GetEnumerator() => new HashcodeDictionaryEnumerator<TKey, TValue>(this);
	}
}