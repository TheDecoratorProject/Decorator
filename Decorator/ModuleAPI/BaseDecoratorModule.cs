using System;

namespace Decorator.ModuleAPI
{
	public abstract class BaseDecoratorModule
	{
		public abstract Type ModifiedType { get; }
		public abstract Type OriginalType { get; }
		public abstract Member Member { get; }
		public abstract IConverterContainer Container { get; }

		public abstract bool Deserialize(object instance, ref object[] array, ref int i);

		public abstract void Serialize(object instance, ref object[] array, ref int i);

		public abstract void EstimateSize(object instance, ref int size);
	}
}