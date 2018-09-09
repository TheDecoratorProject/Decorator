using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Decorator {

	public interface ICache<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> {
		ConcurrentDictionary<TKey, TValue> Cache { get; }

		void Store(TKey key, TValue value);

		TValue Retrieve(TKey key, Func<TValue> lacksKey);
	}

	public class CacheManager<TKey, TValue> : ICache<TKey, TValue> {

		public CacheManager()
			=> this.Cache = new ConcurrentDictionary<TKey, TValue>();

		public ConcurrentDictionary<TKey, TValue> Cache { get; }

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)this.Cache).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)this.Cache).GetEnumerator();

		public TValue Retrieve(TKey key, Func<TValue> lacksKey) {
			if (this.Cache.TryGetValue(key, out var val)) {
				return val;
			} else {
				var result = lacksKey.Invoke();

				if (this.Cache.TryAdd(key, result))
					return result;
				else if (!this.Cache.TryGetValue(key, out val))
					throw new Exception("what the waht D:");

				return val;
			}
		}

		public void Store(TKey key, TValue value) {
			if (!this.Cache.TryAdd(key, value))
				this.Cache[key] = value;
		}
	}
}