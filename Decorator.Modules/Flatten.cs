using Decorator.ModuleAPI;

using System;
using System.Linq;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class FlattenAttribute : Attribute, IModuleBuilder
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public Module<T> Build<T>(BaseContainer modContainer)
		{
			return (Module<T>)typeof(FlattenModule<>)
				.MakeGenericType(typeof(T))
				.GetConstructors()
				.First()
				.Invoke(new object[] { (ConverterContainerContainer)modContainer });
		}

		public class FlattenModule<T> : Module<T>
			where T : new()
		{
			public FlattenModule(ConverterContainerContainer modContainer)
				: base(modContainer)
			{
				_converter = modContainer.Container.RequestConverter<T>();
				_modules = _converter.Members.ToArray();
			}

			private readonly IConverter<T> _converter;
			private readonly BaseModule[] _modules;

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				if (!_converter.TryDeserialize(array, ref i, out var result))
				{
					return false;
				}

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