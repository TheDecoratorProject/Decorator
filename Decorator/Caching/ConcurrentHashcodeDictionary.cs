using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
			bool val;

			val = Dictionary.TryGetValue(key.GetHashCode(), out value);

			return val;
		}

		public IEnumerable<KeyValuePair<TKey, TValue>> GetItems()
		{
			var valenumer = Dictionary.GetEnumerator();
			var keysenumer = DictionaryKeys.GetEnumerator();

			while (valenumer.MoveNext() && keysenumer.MoveNext())
			{
				var kvp = new KeyValuePair<TKey, TValue>(keysenumer.Current.Value, valenumer.Current.Value);

				yield return kvp;
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new HashcodeDictionaryEnumerator<TKey, TValue>(this);

		IEnumerator IEnumerable.GetEnumerator() => new HashcodeDictionaryEnumerator<TKey, TValue>(this);
	}
}