using SwissILKnife;

using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ArrayAttribute : Attribute, IDecorationFactory
	{
		public ArrayAttribute() : this(short.MaxValue)
		{
		}

		public ArrayAttribute(int maxSize) => MaxSize = maxSize;

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

		private ArrayDecoration<T> MakeDecoration<T>(MemberInfo member)
			=> new ArrayDecoration<T>
			(
				MemberUtils.GenerateSetMethod(member),
				MemberUtils.GenerateGetMethod(member),
				MaxSize
			);

		public class ArrayDecoration<T> : IDecoration
		{
			private readonly bool _canBeNull;
			private readonly SetMethod _setMethod;
			private readonly GetMethod _getMethod;
			private readonly int _maxSize;

			public ArrayDecoration(SetMethod setMethod, GetMethod getMethod, int maxSize)
			{
				_setMethod = setMethod;
				_getMethod = getMethod;
				_maxSize = maxSize;
				_canBeNull = !typeof(T).IsValueType; // IsReferenceType
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

			public void Serialize(ref object[] array, object instance, ref int index)
			{
				var arrayVal = (T[])_getMethod(instance);
				array[index++] = arrayVal.Length;

				for (var arrayValInex = 0; arrayValInex < arrayVal.Length; arrayValInex++)
				{
					array[index++] = arrayVal[arrayValInex];
				}
			}

			public bool Deserialize(ref object[] array, object instance, ref int index)
			{
				if (!(array[index] is int len))
				{
					return false;
				}

				if (len > _maxSize || len < 0 ||
					(array.Length <= index + len))
				{
					return false;
				}

				var desArray = new object[len];

				index++;

				for (var desArrayIndex = 0; desArrayIndex < len; desArrayIndex++)
				{
					if (!(array[index] is T ||
						(_canBeNull && array[index] == null)))
					{
						return false;
					}

					desArray[desArrayIndex] = array[index];

					index++;
				}

				_setMethod(instance, desArray);

				return true;
			}

			public void EstimateSize(object instance, ref int size) => size += ((T[])_getMethod(instance)).Length + 1;
		}
	}
}