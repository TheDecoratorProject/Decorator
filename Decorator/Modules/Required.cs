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
			=> new Module<T>(modifiedType, memberInfo);

		public class Module<T> : DecoratorModule<T>
		{
			public Module(Type modifiedType, MemberInfo memberInfo)
				: base(modifiedType, memberInfo)
			{
				if (!modifiedType.IsValueType)
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