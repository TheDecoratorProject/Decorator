using SwissILKnife;

namespace Decorator
{
	public static class Converter<T>
		where T : IDecorable
	{
		private static readonly DecoratorInfo[] _members;

		static Converter() => _members = DecoratorInfoCompiler<T>.Members;

		public static bool Deserialize(object[] array, out T result)
		{
			int arrayIndex = 0;

			return Deserialize(array, ref arrayIndex, out result);
		}

		public static bool Deserialize(object[] array, ref int arrayIndex, out T result)
		{
			result = InstanceOf<T>.Create();

			for (int memberIndex = 0; memberIndex < _members.Length; memberIndex++)
			{
				if (!_members[memberIndex].Deserialize(result, ref array, ref arrayIndex)) return false;
			}

			return true;
		}

		public static object[] Serialize(T item)
		{
			var array = new object[_members.EstimateSize(item)];

			int arrayIndex = 0;

			for (int memberIndex = 0; memberIndex < _members.Length; memberIndex++)
			{
				_members[memberIndex].Serialize(item, ref array, ref arrayIndex);
			}

			return array;
		}
	}
}