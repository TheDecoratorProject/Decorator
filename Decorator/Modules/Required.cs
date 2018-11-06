using Decorator.ModuleAPI;

using System;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class RequiredAttribute : Attribute, IDecoratorModuleBuilder
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public DecoratorModule<T> Build<T>(ModuleContainer modContainer)
			=> modContainer.ModifiedType.IsValueType ?
				new ValueTypeModule<T>(modContainer)
				: (DecoratorModule<T>)new ReferenceTypeModule<T>(modContainer);

		public class ValueTypeModule<T> : DecoratorModule<T>
		{
			public ValueTypeModule(ModuleContainer modContainer)
				: base(modContainer)
			{
			}

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				if (array[i] is T)
				{
					SetValue(instance, array[i]);
					i++;
					return true;
				}

				return false;
			}

			public override void EstimateSize(object instance, ref int size) => size++;

			public override void Serialize(object instance, ref object[] array, ref int i) => array[i++] = GetValue(instance);
		}

		public class ReferenceTypeModule<T> : DecoratorModule<T>
		{
			public ReferenceTypeModule(ModuleContainer modContainer)
				: base(modContainer)
			{
			}

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				if (array[i] == null)
				{
					i++;
					return true;
				}

				if (array[i] is T)
				{
					SetValue(instance, array[i++]);
					return true;
				}

				return false;
			}

			public override void EstimateSize(object instance, ref int size) => size++;

			public override void Serialize(object instance, ref object[] array, ref int i) => array[i++] = GetValue(instance);
		}
	}
}