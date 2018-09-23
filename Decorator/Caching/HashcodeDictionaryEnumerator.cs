using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Decorator.Caching {

	internal class HashcodeDictionaryEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>> {
		public HashcodeDictionaryEnumerator(IHashcodeDictionary<TKey, TValue> hcd) {
			this._hcd = hcd;
			this._enumer = this._hcd.GetItems().GetEnumerator();
		}

		private IHashcodeDictionary<TKey, TValue> _hcd;
		private IEnumerator<KeyValuePair<TKey, TValue>> _enumer;

		public KeyValuePair<TKey, TValue> Current => this._enumer.Current;

		object IEnumerator.Current => this._enumer.Current;

		public bool MoveNext() => this._enumer.MoveNext();
		public void Reset() => this._enumer.Reset();
		public void Dispose() => this._enumer.Dispose();
	}
}
