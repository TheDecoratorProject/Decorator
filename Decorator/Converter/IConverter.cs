using Decorator.ModuleAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Decorator
{
	public interface IConverter
	{
	}

	public interface IConverter<T> : IConverter
		where T : IDecorable, new()
	{
		ReadOnlyCollection<BaseDecoratorModule> Members { get; }

		bool TryDeserialize(object[] array, out T result);

		bool TryDeserialize(object[] array, ref int arrayIndex, out T result);

		object[] Serialize(T item);
	}
}
