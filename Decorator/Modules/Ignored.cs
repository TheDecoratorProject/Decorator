using Decorator.ModuleAPI;

using System;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class IgnoredAttribute : Attribute, IDecoratorModuleBuilder
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public DecoratorModule<T> Build<T>(Type modifiedType, Member member)
			=> new Module<T>(modifiedType, member);

		public class Module<T> : DecoratorModule<T>
		{
			public Module(Type modifiedType, Member member)
				: base(modifiedType, member) => _logic = new IgnoredLogic();

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
			public override Type OriginalType => null;
			public override Type ModifiedType => null;
			public override Member Member => default;

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