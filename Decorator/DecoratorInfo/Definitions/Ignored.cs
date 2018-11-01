using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class IgnoredAttribute : Attribute, IDecoratorInfoAttribute
	{
		DecoratorInfo IDecoratorInfoAttribute.GetDecoratorInfo(MemberInfo memberValue) => new Ignored();
	}

	internal class Ignored : DecoratorInfo
	{
		public override bool Deserialize(object instance, ref object[] array, ref int i)
		{
			i++;
			return true;
		}

		public override void Serialize(object instance, ref object[] array, ref int i) => i++;

		public override void EstimateSize(object instance, ref int i) => i++;
	}
}