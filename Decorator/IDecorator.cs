namespace Decorator
{
	public interface IDecorator<T>
	{
		object[] Serialize(T item);

		object[] Serialize(T item, ref int index);

		bool TryDeserialize(object[] array, out T result);

		bool TryDeserialize(object[] array, ref int index, out T result);

		bool TryDeserialize(object[] array, T instance, ref int index);

		int EstimateSize(T instance);

		void EstimateSize(T instance, ref int index);
	}
}