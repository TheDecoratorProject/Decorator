using SwissILKnife;

using System.Runtime.CompilerServices;

namespace Decorator
{
	public class Decorator<T> : IDecorator<T>
	{
		private readonly IDecoration[] _decorations;

		public Decorator(params IDecoration[] decorations) => _decorations = decorations;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object[] Serialize(T item)
		{
			var index = 0;

			return Serialize(item, ref index);
		}

		public object[] Serialize(T item, ref int index)
		{
			var size = EstimateSize(item);

			var array = new object[size];

			foreach (var decoration in _decorations)
			{
				decoration.Serialize(ref array, item, ref index);
			}

			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryDeserialize(object[] array, out T result)
		{
			var index = 0;

			return TryDeserialize(array, ref index, out result);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryDeserialize(object[] array, ref int index, out T result)
		{
			result = InstanceOf<T>.Create();

			return TryDeserialize(array, result, ref index);
		}

		public bool TryDeserialize(object[] array, T instance, ref int index)
		{
			foreach (var decoration in _decorations)
			{
				if (!decoration.Deserialize(ref array, instance, ref index))
				{
					return false;
				}
			}

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int EstimateSize(T instance)
		{
			var size = 0;

			EstimateSize(instance, ref size);

			return size;
		}

		public void EstimateSize(T instance, ref int size)
		{
			foreach (var decoration in _decorations)
			{
				decoration.EstimateSize(instance, ref size);
			}
		}
	}
}