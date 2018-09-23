using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Decorator {
	internal class HashcodeDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> {
		public HashcodeDictionary() {
			this.Dictionary = new ConcurrentDictionary<int, TValue>();
			this.DictionaryKeys = new ConcurrentDictionary<int, TKey>();
		}

		public ConcurrentDictionary<int, TValue> Dictionary { get; set; }
		public ConcurrentDictionary<int, TKey> DictionaryKeys { get; set; }

		public bool TryAdd(TKey key, TValue value) {
			var hashcode = key.GetHashCode();

			return this.Dictionary.TryAdd(hashcode, value) &&
					this.DictionaryKeys.TryAdd(hashcode, key);
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

	internal class HashcodeDictionaryEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>> {
		public HashcodeDictionaryEnumerator(HashcodeDictionary<TKey, TValue> hcd) {
			this._hcd = hcd;
			this._enumer = this._hcd.GetItems().GetEnumerator();
		}

		private HashcodeDictionary<TKey, TValue> _hcd;
		private IEnumerator<KeyValuePair<TKey, TValue>> _enumer;

		public KeyValuePair<TKey, TValue> Current => this._enumer.Current;

		object IEnumerator.Current => this._enumer.Current;

		public bool MoveNext() => this._enumer.MoveNext();
		public void Reset() => this._enumer.Reset();
		public void Dispose() => this._enumer.Dispose();
	}
}