using Decorator.ModuleAPI;

using System;

namespace Decorator.Modules
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class IgnoredAttribute : Attribute, IModuleAttribute
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public Module<T> Build<T>(BaseContainer modContainer)
			=> new IgnoredModule<T>(modContainer);

		public class IgnoredModule<T> : Module<T>
		{
			public IgnoredModule(BaseContainer modContainer)
				: base(modContainer) => _logic = new IgnoredLogic();

			private readonly IgnoredLogic _logic;

			public override bool Deserialize(object instance, ref object[] array, ref int i)
				=> _logic.Deserialize(instance, ref array, ref i);

			public override void Serialize(object instance, ref object[] array, ref int i)
				=> _logic.Serialize(instance, ref array, ref i);

			public override void EstimateSize(object instance, ref int i)
				=> _logic.EstimateSize(instance, ref i);
		}
	}
}