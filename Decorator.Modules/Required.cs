using Decorator.ModuleAPI;

using System;

namespace Decorator.Modules
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class RequiredAttribute : Attribute, IModuleAttribute
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public Module<T> Build<T>(BaseContainer modContainer)
			=> typeof(T).IsValueType ?
				new RequiredValueTypeModule<T>(modContainer)
				: (Module<T>)new RequiredReferenceTypeModule<T>(modContainer);

		public class RequiredValueTypeModule<T> : Module<T>
		{
			public RequiredValueTypeModule(BaseContainer modContainer)
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
			public RequiredReferenceTypeModule(BaseContainer modContainer)
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