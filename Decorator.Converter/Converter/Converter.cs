using Decorator.ModuleAPI;

using SwissILKnife;

using System.Collections.ObjectModel;

namespace Decorator.Converter
{
	public class Converter<T> : IConverter<T>
		where T : new()
	{
		public Converter(BaseModule[] members)
		{
			_members = members;
			Members = new ReadOnlyCollection<BaseModule>(_members);
		}

		private readonly BaseModule[] _members;

		public ReadOnlyCollection<BaseModule> Members { get; }

		public object[] Serialize(T item)
		{
			var array = new object[_members.EstimateSize(item)];

			var arrayIndex = 0;

			for (var memberIndex = 0; memberIndex < _members.Length; memberIndex++)
			{
				_members[memberIndex].Serialize(item, ref array, ref arrayIndex);
			}

			return array;
		}

		public bool TryDeserialize(object[] array, out T result)
		{
			if (array == null)
			{
				result = default;
				return false;
			}

			var arrayIndex = 0;

			return TryDeserialize(array, ref arrayIndex, out result);
		}

		public bool TryDeserialize(object[] array, ref int arrayIndex, out T result)
		{
			result = InstanceOf<T>.Create();

			for (var memberIndex = 0; memberIndex < _members.Length; memberIndex++)
			{
				if (array.Length <= arrayIndex) return false;

				if (!_members[memberIndex].Deserialize(result, ref array, ref arrayIndex)) return false;
			}

			return true;
		}
	}
}