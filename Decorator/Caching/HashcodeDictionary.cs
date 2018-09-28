using System.Collections;
using System.Collections.Generic;

namespace Decorator.Caching
{
	internal class HashcodeDictionary<TKey, TValue> : IHashcodeDictionary<TKey, TValue>
	{
		public HashcodeDictionary()
		{
			Dictionary = new ResizingArray<TValue>();
			DictionaryKeys = new ResizingArray<TKey>();
		}

		public ResizingArray<TValue> Dictionary { get; set; }
		public ResizingArray<TKey> DictionaryKeys { get; set; }

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
			var arr1 = Dictionary.Array;
			var arr2 = DictionaryKeys.Array;

			for (int i = 0; i < Dictionary.Length; i++)
			{
				yield return new KeyValuePair<TKey, TValue>(
						arr2[i],
						arr1[i]
					);
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new HashcodeDictionaryEnumerator<TKey, TValue>(this);

		IEnumerator IEnumerable.GetEnumerator() => new HashcodeDictionaryEnumerator<TKey, TValue>(this);
	}
}