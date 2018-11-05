using Decorator.ModuleAPI;

using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class IgnoredAttribute : Attribute, IDecoratorModuleBuilder
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public DecoratorModule<T> Build<T>(Type modifiedType, MemberInfo memberInfo)
			=> new Module<T>(modifiedType, memberInfo);

		public class Module<T> : DecoratorModule<T>
		{
			public Module(Type modifiedType, MemberInfo memberInfo)
				: base(modifiedType, memberInfo) => _logic = new IgnoredLogic();

			private IgnoredLogic _logic;

			public override bool Deserialize(object instance, ref object[] array, ref int i)
				=> _logic.Deserialize(instance, ref array, ref i);

			public override void Serialize(object instance, ref object[] array, ref int i)
				=> _logic.Serialize(instance, ref array, ref i);

			public override void EstimateSize(object instance, ref int i)
				=> _logic.EstimateSize(instance, ref i);
		}

		internal class IgnoredLogic : BaseDecoratorModule
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
}

/*using System;
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
}*/