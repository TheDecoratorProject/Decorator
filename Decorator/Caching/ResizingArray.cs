namespace Decorator.Caching
{
	internal class ResizingArray<T>
	{
		public ResizingArray(int size = 0x1000)
		{
			_array = new T[size];
			_keys = new int[size];
			_counter = 0;
		}

		private int _counter;
		private T[] _array;
		private int[] _keys;

		public T[] Array => _array;

		public int Length => _counter;

		public void AddItem(int key, T item)
		{
			if (_array.Length >= _counter) ResizeArray();

			_array[_counter] = item;
			_keys[_counter] = key;

			_counter++;
		}

		public bool TryGetValue(int key, out T item)
		{
			for (int i = 0; i < _keys.Length; i++)
			{
				if (key == _keys[i])
				{
					item = _array[i];
					return true;
				}
			}

			item = default;
			return false;
		}

		public T this[int key]
		{
			set => AddItem(key, value);
		}

		private void ResizeArray()
		{
			var newLen = _array.Length * 2;
			System.Array.Resize(ref _array, newLen);
			System.Array.Resize(ref _keys, newLen);
		}
	}
}