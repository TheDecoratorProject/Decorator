using Decorator.ModuleAPI;

using System;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class OptionalAttribute : Attribute, IModuleBuilder
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public Module<T> Build<T>(BaseContainer modContainer)
			=> new OptionalModule<T>(modContainer);

		public class OptionalModule<T> : Module<T>
		{
			public OptionalModule(BaseContainer modContainer)
				: base(modContainer)
			{
				if (!typeof(T).IsValueType)
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