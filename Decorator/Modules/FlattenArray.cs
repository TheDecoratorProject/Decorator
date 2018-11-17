﻿using Decorator.ModuleAPI;

using System;
using System.Linq;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class FlattenArrayAttribute : Attribute, IModuleBuilder
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

			var elementType = attributeAppliedTo.GetElementType();

			if (!elementType.GetInterfaces()
									.Contains(typeof(IDecorable)))
			{
				throw new InvalidDeclarationException($"{attributeAppliedTo} does not (properly) inherit from {typeof(IDecorable)}.");
			}

			return elementType;
		}

		public Module<T> Build<T>(ModuleContainer modContainer)
		{
			return (Module<T>)typeof(FlattenArrayModule<>)
				.MakeGenericType(typeof(T))
				.GetConstructors()
				.First()
				.Invoke(new object[] { modContainer, MaxArraySize });
		}

		public class FlattenArrayModule<T> : Module<T>
			where T : IDecorable, new()
		{
			public FlattenArrayModule(ModuleContainer modContainer, int arraySize)
				: base(modContainer)
			{
				_converter = ModuleContainer.Container.RequestConverter<T>();
				_modules = _converter.Members.ToArray();
				_maxSize = arraySize;
			}

			private readonly IConverter<T> _converter;
			private readonly BaseModule[] _modules;
			private readonly int _maxSize;

			//TODO: Unit test to ensure DConverter<T> ism't being called, and _converter is

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