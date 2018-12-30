namespace Decorator
{
	public class Ignored : IDecoration
	{
		public int Size { get; }

		public Ignored(int size = 1) => Size = size;

		public void Serialize(ref object[] array, object instance, ref int index) => index += Size;

		public bool Deserialize(ref object[] array, object instance, ref int index)
		{
			index += Size;
			return true;
		}

		public void EstimateSize(object instance, ref int size) => size += Size;
	}
}