using Decorator.Converter;
using Decorator.ModuleAPI;

using System.Collections.ObjectModel;

namespace Decorator
{
	public static class DConverter<T>
		where T : new()
	{
		private static readonly IConverter<T> _converter;

		static DConverter()
		{
			_converter = StaticProvider.Container.RequestConverter<T>();
			Members = _converter.Members;
		}

		public static ReadOnlyCollection<BaseModule> Members { get; }

		public static bool TryDeserialize(object[] array, out T result)
			=> _converter.TryDeserialize(array, out result);

		public static bool TryDeserialize(object[] array, ref int arrayIndex, out T result)
			=> _converter.TryDeserialize(array, ref arrayIndex, out result);

		public static object[] Serialize(T item)
			=> _converter.Serialize(item);
	}
}