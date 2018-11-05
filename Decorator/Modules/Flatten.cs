using Decorator.ModuleAPI;

using System;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class FlattenAttribute : Attribute, IDecoratorModuleBuilder, IDecoratorDecorableModuleBuilder
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public DecoratorModule<T> Build<T>(Type modifiedType, Member member) => EnsureIDecorable<FlattenAttribute>.InvokeBuild<T>(this, modifiedType, member);

		public DecoratorModule<T> BuildDecorable<T>(Type modifiedType, Member member)
			where T : IDecorable => new Module<T>(modifiedType, member);

		public class Module<T> : DecoratorModule<T>
			where T : IDecorable
		{
			public Module(Type modifiedType, Member member)
				: base(modifiedType, member)
			{
			}

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				if (!Converter<T>.TryDeserialize(array, ref i, out var result)) return false;
				SetValue(instance, result);
				return true;
			}

			public override void Serialize(object instance, ref object[] array, ref int i)
			{
				var data = Converter<T>.Serialize((T)GetValue(instance));

				for (var arrayIndex = 0; arrayIndex < data.Length; arrayIndex++)
				{
					array[i++] = data[arrayIndex];
				}
			}

			public override void EstimateSize(object instance, ref int i)
				=> i += DecoratorModuleContainer<T>.MembersValue.EstimateSize((T)GetValue(instance));
		}
	}
}