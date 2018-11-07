using Decorator.ModuleAPI;

using System;
using System.Linq;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class FlattenArrayAttribute : Attribute, IDecoratorModuleBuilder, IDecoratorDecorableModuleBuilder
	{
		public FlattenArrayAttribute() : this(0xFFFF)
		{
		}

		public FlattenArrayAttribute(int arraySize)
			=> MaxArraySize = arraySize;

		public int MaxArraySize { get; set; }

		public Type ModifyAppliedType(Type attributeAppliedTo)
		{
			if (!attributeAppliedTo.IsArray)
			{
				throw new InvalidDeclarationException($"{attributeAppliedTo} must be an array!");
			}

			return attributeAppliedTo.GetElementType();
		}

		public DecoratorModule<T> Build<T>(ModuleContainer modContainer) => EnsureIDecorable<FlattenArrayAttribute>.InvokeBuild<T>(this, modContainer);

		public DecoratorModule<T> BuildDecorable<T>(ModuleContainer modContainer)
			where T : IDecorable, new() => new Module<T>(modContainer, MaxArraySize);

		public class Module<T> : DecoratorModule<T>
			where T : IDecorable, new()
		{
			public Module(ModuleContainer modContainer, int arraySize)
				: base(modContainer)
			{
				_converter = ModuleContainer.Container.Request<T>();
				_modules = _converter.Members.ToArray();
				_maxSize = arraySize;
			}

			private IConverter<T> _converter;
			private BaseDecoratorModule[] _modules;
			private readonly int _maxSize;

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				if (array[i] is int len)
				{
					i++;

					if (len > _maxSize || len < 0) return false;

					var desArray = new object[len];

					for (var desArrayIndex = 0; desArrayIndex < len; desArrayIndex++)
					{
						if (!DConverter<T>.TryDeserialize(array, ref i, out var item))
						{
							return false;
						}

						desArray[desArrayIndex] = item;
					}

					SetValue(instance, desArray);

					return true;
				}

				return false;
			}

			public override void Serialize(object instance, ref object[] array, ref int i)
			{
				var arrayVal = (T[])GetValue(instance);

				array[i++] = arrayVal.Length;

				for (var arrayValIndex = 0; arrayValIndex < arrayVal.Length; arrayValIndex++)
				{
					var data = DConverter<T>.Serialize(arrayVal[arrayValIndex]);

					for (var arrayIndex = 0; arrayIndex < data.Length; arrayIndex++)
					{
						array[i++] = data[arrayIndex];
					}
				}
			}

			public override void EstimateSize(object instance, ref int i)
			{
				var array = (T[])GetValue(instance);

				i++;

				for (var arrayIndex = 0; arrayIndex < array.Length; arrayIndex++)
				{
					i += _modules.EstimateSize(array[arrayIndex]);
				}
			}
		}
	}
}