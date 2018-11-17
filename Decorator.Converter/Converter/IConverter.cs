using Decorator.ModuleAPI;

using System.Collections.ObjectModel;

namespace Decorator.Converter
{
	public interface IConverter<T>
		where T : new()
	{
		ReadOnlyCollection<BaseModule> Members { get; }

		bool TryDeserialize(object[] array, out T result);

		bool TryDeserialize(object[] array, ref int arrayIndex, out T result);

		object[] Serialize(T item);
	}
}