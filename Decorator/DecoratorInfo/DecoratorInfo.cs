using SwissILKnife;

using System;
using System.Reflection;

namespace Decorator
{
	internal abstract class DecoratorInfo
	{
		public abstract bool Deserialize(object instance, ref object[] array, ref int i);

		public abstract void Serialize(object instance, ref object[] array, ref int i);

		public abstract void EstimateSize(object instance, ref int gaugeSize);
	}
}