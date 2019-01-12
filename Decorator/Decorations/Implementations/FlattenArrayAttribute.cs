using SwissILKnife;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class FlattenArrayAttribute : Attribute, IDecorationFactory
	{
		public FlattenArrayAttribute() : this(short.MaxValue)
		{
		}

		public FlattenArrayAttribute(int maxSize) => MaxSize = maxSize;

		public int MaxSize { get; set; }

		public Type GetType(PropertyInfo property) => ArrayIntoUnit(property.PropertyType);

		public Type GetType(FieldInfo field) => ArrayIntoUnit(field.FieldType);

		public IDecoration Make<T>(PropertyInfo property) => MakeDecoration<T>(property);

		public IDecoration Make<T>(FieldInfo field) => MakeDecoration<T>(field);

		private Type ArrayIntoUnit(Type input)
		{
			if (!input.IsArray)
			{
				throw new NotAnArrayException();
			}

			// input is a T[]
			return input.GetElementType();
		}

		private FlattenArrayDecoration<T> MakeDecoration<T>(MemberInfo member)
			=> new FlattenArrayDecoration<T>
			(
				MemberUtils.GenerateSetMethod(member),
				MemberUtils.GenerateGetMethod(member),
				MaxSize
			);

		public class FlattenArrayDecoration<T> : IDecoration
		{
			private readonly bool _canBeNull;
			private readonly Decorator<T> _decorator;
			private readonly SetMethod _setMethod;
			private readonly GetMethod _getMethod;
			private readonly int _maxSize;

			public FlattenArrayDecoration(SetMethod setMethod, GetMethod getMethod, int maxSize)
			{
				_setMethod = setMethod;
				_getMethod = getMethod;
				_maxSize = maxSize;
				_canBeNull = !typeof(T).IsValueType; // IsReferenceType
				_decorator = DDecorator<T>.Instance;
			}

			// =-------=
			//  WARNING
			// =-------=

			// the follow code is old and copied and pasted from the previous decorator.
			// if you want to change it or modify it at all, please add unit tests first.
			// i won't be mean, here's the commit to use as a reference
			// https://github.com/SirJosh3917/Decorator/commit/daa95c9b9279388d37a28146bbd33613e59bd28b

			// i'm pretty much relying on the fact that i tested these thoroughly in the previous decorator
			// so if you're reading this and don't know how to help out... :D?

			public bool Deserialize(ref object[] array, object instance, ref int index)
			{
				if (!(array[index] is int len))
				{
					return false;
				}

				index++;

				if (len > _maxSize || len < 0) return false;

				var desArray = new object[len];

				for (var desArrayIndex = 0; desArrayIndex < len; desArrayIndex++)
				{
					if (!_decorator.TryDeserialize(array, ref index, out var item))
					{
						return false;
					}

					desArray[desArrayIndex] = item;
				}

				_setMethod(instance, desArray);

				return true;
			}

			public void Serialize(ref object[] array, object instance, ref int index)
			{
				var arrayVal = (T[])_getMethod(instance);

				array[index++] = arrayVal.Length;

				for (var arrayValIndex = 0; arrayValIndex < arrayVal.Length; arrayValIndex++)
				{
					var data = _decorator.Serialize(arrayVal[arrayValIndex]);

					for (var arrayIndex = 0; arrayIndex < data.Length; arrayIndex++)
					{
						array[index++] = data[arrayIndex];
					}
				}
			}

			public void EstimateSize(object instance, ref int i)
			{
				var array = (T[])_getMethod(instance);

				i++;

				for (var arrayIndex = 0; arrayIndex < array.Length; arrayIndex++)
				{
					i += _decorator.EstimateSize(array[arrayIndex]);
				}
			}
		}
	}
}
