using Decorator.Compiler;
using Decorator.ModuleAPI;
using System;
using System.Collections.ObjectModel;

namespace Decorator.Converter
{
	public class ILConverter<T> : IILConverter<T>
		where T : new()
	{
		public ILConverter(BaseModule[] members, ILSerialize<T> ilSerialize, ILDeserialize<T> ilDeserialize)
		{
			_serialize = ilSerialize;
			_deserialize = ilDeserialize;

			_members = members;
			Members = new ReadOnlyCollection<BaseModule>(_members);
		}

		private readonly ILSerialize<T> _serialize;
		private readonly ILDeserialize<T> _deserialize;

		public ILSerialize<T> Serialize => _serialize;
		public ILDeserialize<T> Deserialize => _deserialize;

		private readonly BaseModule[] _members;
		public ReadOnlyCollection<BaseModule> Members { get; }

		public bool TryDeserialize(object[] array, out T result)
		{
			if( array == null)
			{
				result = default;
				return false;
			}

			int i = 0;
			return TryDeserialize(array, ref i, out result);
		}

		public bool TryDeserialize(object[] array, ref int arrayIndex, out T result)
			=> _deserialize(array, ref arrayIndex, out result);

		object[] IConverter<T>.Serialize(T item)
			=> _serialize(item);
	}
}