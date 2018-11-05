using SwissILKnife;

namespace Decorator
{
	public static class Converter<T>
		where T : IDecorable
	{
		private static readonly ModuleAPI.BaseDecoratorModule[] _members;

		static Converter()
		{
			_members = DecoratorModuleContainer<T>.MembersValue;
		}

		public static bool TryDeserialize(object[] array, out T result)
		{
			if (array == null)
			{
				result = default;
				return false;
			}

			var arrayIndex = 0;

			return TryDeserialize(array, ref arrayIndex, out result);
		}

		public static bool TryDeserialize(object[] array, ref int arrayIndex, out T result)
		{
			result = InstanceOf<T>.Create();

			for (var memberIndex = 0; memberIndex < _members.Length; memberIndex++)
			{
				if (array.Length <= arrayIndex) return false;

				if (!_members[memberIndex].Deserialize(result, ref array, ref arrayIndex)) return false;
			}

			return true;
		}

		public static object[] Serialize(T item)
		{
			var array = new object[_members.EstimateSize(item)];

			var arrayIndex = 0;

			for (var memberIndex = 0; memberIndex < _members.Length; memberIndex++)
			{
				_members[memberIndex].Serialize(item, ref array, ref arrayIndex);
			}

			return array;
		}
	}
}