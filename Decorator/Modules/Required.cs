using Decorator.ModuleAPI;

using System;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class RequiredAttribute : Attribute, IModuleBuilder
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public Module<T> Build<T>(ModuleContainer modContainer)
			=> modContainer.ModifiedType.IsValueType ?
				(Module<T>)new RequiredValueTypeModule<T>(modContainer)
				: (Module<T>)new RequiredReferenceTypeModule<T>(modContainer);

		public class RequiredValueTypeModule<T> : Module<T>
		{
			public RequiredValueTypeModule(ModuleContainer modContainer)
				: base(modContainer)
			{
			}

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				if (!(array[i] is T))
				{
					return false;
				}

				SetValue(instance, array[i]);
				i++;

				return true;
			}

			public override void EstimateSize(object instance, ref int size) => size++;

			public override void Serialize(object instance, ref object[] array, ref int i) => array[i++] = GetValue(instance);
		}

		public class RequiredReferenceTypeModule<T> : Module<T>
		{
			public RequiredReferenceTypeModule(ModuleContainer modContainer)
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