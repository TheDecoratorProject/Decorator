using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Decorator {

	public interface ICache<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> {
		Dictionary<TKey, TValue> CacheStorage { get; }

		TValue Retrieve(TKey key, Func<TValue> lacksKey);
	}

	public class CacheManager<TKey, TValue> : ICache<TKey, TValue> {

		public CacheManager()
			=> this.CacheStorage = new Dictionary<TKey, TValue>();

		public Dictionary<TKey, TValue> CacheStorage { get; }

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)this.CacheStorage).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)this.CacheStorage).GetEnumerator();

		public TValue Retrieve(TKey key, Func<TValue> lacksKey) {
			if (this.CacheStorage.TryGetValue(key, out var val))
				return val;
			val = lacksKey();
			this.CacheStorage[key] = val;
			return val;
		}
	}
}