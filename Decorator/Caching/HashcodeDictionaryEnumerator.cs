using System.Collections;
using System.Collections.Generic;

namespace Decorator.Caching
{
	internal class HashcodeDictionaryEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
	{
		public HashcodeDictionaryEnumerator(IHashcodeDictionary<TKey, TValue> hcd)
		{
			_hcd = hcd;
			_enumer = _hcd.GetItems().GetEnumerator();
		}

		private readonly IHashcodeDictionary<TKey, TValue> _hcd;
		private readonly IEnumerator<KeyValuePair<TKey, TValue>> _enumer;

		public KeyValuePair<TKey, TValue> Current => _enumer.Current;

		object IEnumerator.Current => _enumer.Current;

		public bool MoveNext() => _enumer.MoveNext();

		public void Reset() => _enumer.Reset();

		public void Dispose() => _enumer.Dispose();
	}
}