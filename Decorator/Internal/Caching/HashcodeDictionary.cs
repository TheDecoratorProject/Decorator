using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Decorator.Caching
{
	internal class HashcodeDictionary<TKey, TValue> : IHashcodeDictionary<TKey, TValue>
	{
		public HashcodeDictionary()
		{
			Dictionary = new Dictionary<int, TValue>();
			DictionaryKeys = new Dictionary<int, TKey>();
		}

		public Dictionary<int, TValue> Dictionary { get; set; }
		public Dictionary<int, TKey> DictionaryKeys { get; set; }

		public bool TryAdd(TKey key, TValue value)
		{
			var hashcode = key.GetHashCode();

			Dictionary[hashcode] = value;
			DictionaryKeys[hashcode] = key;

			return true;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			bool val;

			val = Dictionary.TryGetValue(key.GetHashCode(), out value);

			return val;
		}

		public IEnumerable<KeyValuePair<TKey, TValue>> GetItems()
		{
			return Dictionary.Zip(DictionaryKeys, (value, key) => {
				return new KeyValuePair<TKey, TValue>(key.Value, value.Value);
			});
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new HashcodeDictionaryEnumerator<TKey, TValue>(this);

		IEnumerator IEnumerable.GetEnumerator() => new HashcodeDictionaryEnumerator<TKey, TValue>(this);
	}
}