using Decorator.ModuleAPI;

using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class OptionalAttribute : Attribute, IDecoratorModuleBuilder
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public DecoratorModule<T> Build<T>(Type modifiedType, Member member)
			=> new Module<T>(modifiedType, member);

		public class Module<T> : DecoratorModule<T>
		{
			public Module(Type modifiedType, Member member)
				: base(modifiedType, member)
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