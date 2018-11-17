namespace Decorator.ModuleAPI
{
	public abstract class BaseModule
	{
		public abstract BaseContainer ModuleContainer { get; }

		public abstract bool Deserialize(object instance, ref object[] array, ref int i);

		public abstract void Serialize(object instance, ref object[] array, ref int i);

		public abstract void EstimateSize(object instance, ref int size);
	}
}