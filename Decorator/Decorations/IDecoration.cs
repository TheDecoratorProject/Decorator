namespace Decorator
{
	public interface IDecoration
	{
		// 'instance' is the instance of the object being serialized
		void Serialize(ref object[] array, object instance, ref int index);

		bool Deserialize(ref object[] array, object instance, ref int index);

		void EstimateSize(object instance, ref int size);
	}
}