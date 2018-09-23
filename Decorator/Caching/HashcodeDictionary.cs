using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Decorator.Caching {
	internal class HashcodeDictionary<TKey, TValue> : IHashcodeDictionary<TKey, TValue> {
		public HashcodeDictionary() {
			this.Dictionary = new Dictionary<int, TValue>();
			this.DictionaryKeys = new Dictionary<int, TKey>();
		}

		public HashcodeDictionary(Dictionary<TKey, TValue> dict) : this() {
			foreach (var i in dict) {
				var hc = i.Key.GetHashCode();

				this.Dictionary[hc] = i.Value;
				this.DictionaryKeys[hc] = i.Key;
			}
		}

		public Dictionary<int, TValue> Dictionary { get; set; }
		public Dictionary<int, TKey> DictionaryKeys { get; set; }

		public bool TryAdd(TKey key, TValue value) {
			var hashcode = key.GetHashCode();

			this.Dictionary[hashcode] = value;
			this.DictionaryKeys[hashcode] = key;

			return true;
		}

		public bool TryGetValue(TKey key, out TValue value) {
			bool val;

			val = this.Dictionary.TryGetValue(key.GetHashCode(), out value);

			return val;
		}

		public IEnumerable<KeyValuePair<TKey, TValue>> GetItems() {
			var valenumer = Dictionary.GetEnumerator();
			var keysenumer = DictionaryKeys.GetEnumerator();

			while (valenumer.MoveNext() && keysenumer.MoveNext()) {
				var kvp = new KeyValuePair<TKey, TValue>(keysenumer.Current.Value, valenumer.Current.Value);

				yield return kvp;
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new HashcodeDictionaryEnumerator<TKey, TValue>(this);
		IEnumerator IEnumerable.GetEnumerator() => new HashcodeDictionaryEnumerator<TKey, TValue>(this);
	}
}
