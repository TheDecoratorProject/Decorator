using Decorator.ModuleAPI;

using System;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class RequiredAttribute : Attribute, IDecoratorModuleBuilder
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public DecoratorModule<T> Build<T>(Type modifiedType, Member member)
			=> modifiedType.IsValueType ?
				new ValueTypeModule<T>(modifiedType, member)
				: (DecoratorModule<T>)new ReferenceTypeModule<T>(modifiedType, member);

		public class ValueTypeModule<T> : DecoratorModule<T>
		{
			public ValueTypeModule(Type modifiedType, Member member)
				: base(modifiedType, member)
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
			public ReferenceTypeModule(Type modifiedType, Member member)
				: base(modifiedType, member)
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