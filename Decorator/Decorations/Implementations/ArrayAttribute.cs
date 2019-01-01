using SwissILKnife;

using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ArrayAttribute : Attribute, IDecorationFactory
	{
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
				MemberUtils.GenerateGetMethod(member)
			);

		public class ArrayDecoration<T> : IDecoration
		{
			private readonly bool _canBeNull;
			private readonly SetMethod _setMethod;
			private readonly GetMethod _getMethod;

			public ArrayDecoration(SetMethod setMethod, GetMethod getMethod)
			{
				_setMethod = setMethod;
				_getMethod = getMethod;
				_canBeNull = !typeof(T).IsValueType; // IsReferenceType
			}

			public void Serialize(ref object[] array, object instance, ref int index)
			{
			}

			public bool Deserialize(ref object[] array, object instance, ref int index)
			{
				return false;
			}

			public void EstimateSize(object instance, ref int size)
			{
			}
		}
	}
}