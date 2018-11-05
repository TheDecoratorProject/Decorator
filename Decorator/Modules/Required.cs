using Decorator.ModuleAPI;

using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class RequiredAttribute : Attribute, IDecoratorModuleBuilder
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public DecoratorModule<T> Build<T>(Type modifiedType, MemberInfo memberInfo)
			=> modifiedType.IsValueType ?
				(DecoratorModule<T>)new ValueTypeModule<T>(modifiedType, memberInfo)
				: (DecoratorModule<T>)new ReferenceTypeModule<T>(modifiedType, memberInfo);

		public class ValueTypeModule<T> : DecoratorModule<T>
		{
			public ValueTypeModule(Type modifiedType, MemberInfo memberInfo)
				: base(modifiedType, memberInfo)
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
			public ReferenceTypeModule(Type modifiedType, MemberInfo memberInfo)
				: base(modifiedType, memberInfo)
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