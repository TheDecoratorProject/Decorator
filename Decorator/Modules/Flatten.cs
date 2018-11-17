using Decorator.ModuleAPI;

using System;
using System.Linq;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class FlattenAttribute : Attribute, IDecoratorModuleBuilder
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
		{
			if (!attributeAppliedTo.GetInterfaces()
									.Contains(typeof(IDecorable)))
			{
				throw new InvalidDeclarationException($"{attributeAppliedTo} does not (properly) inherit from {typeof(IDecorable)}.");
			}

			return attributeAppliedTo;
		}

		public DecoratorModule<T> Build<T>(ModuleContainer modContainer)
		{
			return (DecoratorModule<T>)typeof(Module<>)
				.MakeGenericType(typeof(T))
				.GetConstructors()
				.First()
				.Invoke(new object[] { modContainer });
		}
		
		public class Module<T> : DecoratorModule<T>
			where T : IDecorable, new()
		{
			public Module(ModuleContainer modContainer)
				: base(modContainer)
			{
				_converter = ModuleContainer.Container.Request<T>();
				_modules = _converter.Members.ToArray();
			}

			private readonly IConverter<T> _converter;
			private readonly BaseDecoratorModule[] _modules;

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				if (!_converter.TryDeserialize(array, ref i, out var result)) return false;
				SetValue(instance, result);
				return true;
			}

			public override void Serialize(object instance, ref object[] array, ref int i)
			{
				var data = _converter.Serialize((T)GetValue(instance));

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