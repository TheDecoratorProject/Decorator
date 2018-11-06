using Decorator.ModuleAPI;

using System;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class OptionalAttribute : Attribute, IDecoratorModuleBuilder
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public DecoratorModule<T> Build<T>(ModuleContainer modContainer)
			=> new Module<T>(modContainer);

		public class Module<T> : DecoratorModule<T>
		{
			public Module(ModuleContainer modContainer)
				: base(modContainer)
			{
				if (!ModifiedType.IsValueType)
				{
					_canBeNull = true;
				}
			}

			private readonly bool _canBeNull;

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				if (array[i] is T ||
					(_canBeNull &&
					array[i] == null))
				{
					SetValue(instance, array[i]);
				}

				i++;

				return true;
			}

			public override void Serialize(object instance, ref object[] array, ref int i) => array[i++] = GetValue(instance);

			public override void EstimateSize(object instance, ref int i) => i++;
		}
	}
}