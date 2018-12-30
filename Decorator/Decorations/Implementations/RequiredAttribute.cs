using SwissILKnife;

using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class RequiredAttribute : Attribute, IDecorationFactory
	{
		public Type GetType(PropertyInfo property) => property.PropertyType;

		public Type GetType(FieldInfo field) => field.FieldType;

		public IDecoration Make<T>(PropertyInfo property) => MakeDecoration<T>(property);

		public IDecoration Make<T>(FieldInfo field) => MakeDecoration<T>(field);

		private RequiredDecoration<T> MakeDecoration<T>(MemberInfo member)
			=> new RequiredDecoration<T>
			(
				MemberUtils.GetSetMethod(member),
				MemberUtils.GetGetMethod(member)
			);

		public class RequiredDecoration<T> : IDecoration
		{
			private readonly bool _canBeNull;
			private readonly Action<object, object> _setMethod;
			private readonly Func<object, object> _getMethod;

			public RequiredDecoration(Action<object, object> setMethod, Func<object, object> getMethod)
			{
				_setMethod = setMethod;
				_getMethod = getMethod;
				_canBeNull = !typeof(T).IsValueType; // IsReferenceType
			}

			public void Serialize(ref object[] array, object instance, ref int index) => array[index++] = _getMethod(instance);

			public bool Deserialize(ref object[] array, object instance, ref int index)
			{
				if (array.Length <= index)
				{
					return false;
				}

				var obj = array[index++];

				if (obj == null &&
					_canBeNull)
				{
					goto skipTypeCheck;
				}

				if (!(obj is T))
				{
					return false;
				}

			skipTypeCheck:

				_setMethod(instance, obj);
				return true;
			}

			public void EstimateSize(object instance, ref int size) => size++;
		}
	}
}