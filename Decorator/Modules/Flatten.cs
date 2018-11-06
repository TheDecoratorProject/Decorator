using Decorator.ModuleAPI;

using System;
using System.Linq;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class FlattenAttribute : Attribute, IDecoratorModuleBuilder, IDecoratorDecorableModuleBuilder
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public DecoratorModule<T> Build<T>(ModuleContainer modContainer) => EnsureIDecorable<FlattenAttribute>.InvokeBuild<T>(this, modContainer);

		public DecoratorModule<T> BuildDecorable<T>(ModuleContainer modContainer)
			where T : IDecorable, new() => new Module<T>(modContainer);

		public class Module<T> : DecoratorModule<T>
			where T : IDecorable, new()
		{
			public Module(ModuleContainer modContainer)
				: base(modContainer)
			{
				_converter = Container.Request<T>();
				_modules = _converter.Members.ToArray();
			}

			private IConverter<T> _converter;
			private BaseDecoratorModule[] _modules;

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				if (!DConverter<T>.TryDeserialize(array, ref i, out var result)) return false;
				SetValue(instance, result);
				return true;
			}

			public override void Serialize(object instance, ref object[] array, ref int i)
			{
				var data = DConverter<T>.Serialize((T)GetValue(instance));

				for (var arrayIndex = 0; arrayIndex < data.Length; arrayIndex++)
				{
					array[i++] = data[arrayIndex];
				}
			}

			public override void EstimateSize(object instance, ref int i)
				=> i += _modules.EstimateSize((T)GetValue(instance));
		}
	}
}