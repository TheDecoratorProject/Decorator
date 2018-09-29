namespace Decorator.Caching
{
	internal class ResizingArray<T>
	{
		public ResizingArray(int size = 0x1000)
		{
			_array = new T[size];
			_keys = new int[size];
			Length = 0;
		}

		private T[] _array;
		private int[] _keys;

		public T[] Array => _array;

		public int Length { get; private set; }

		public void AddItem(int key, T item)
		{
			if (_array.Length >= Length) ResizeArray();

			_array[Length] = item;
			_keys[Length] = key;

			Length++;
		}

		public bool TryGetValue(int key, out T item)
		{
			for (var i = 0x0; i < _keys.Length; i++)
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