using SwissILKnife;

using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ArrayAttribute : Attribute, IDecoratorInfoAttribute
	{
		public ArrayAttribute() : this(0xFFFF)
		{
		}

		public ArrayAttribute(int maxArraySize)
			=> MaxArraySize = maxArraySize;

		public int MaxArraySize {get; set;}

		DecoratorInfo IDecoratorInfoAttribute.GetDecoratorInfo(MemberInfo memberValue)
		{
			return Make.Class(typeof(Array<>)).Generic(memberValue.GetMemberType().GetElementType())
					.CreateDecoratorInfo(memberValue, MaxArraySize);
		}
	}

	internal class Array<T> : DecoratorInfo
	{
		public Array(MemberInfo memberInfo, int maxSize)
		{
			_maxSize = maxSize;
			_getValue = MemberUtils.GetGetMethod(memberInfo);
			_setValue = MemberUtils.GetSetMethod(memberInfo);
			_canBeNull = !memberInfo.GetMemberType().GetElementType().IsValueType;
		}

		protected Func<object, object> _getValue;
		protected Action<object, object> _setValue;
		private readonly bool _canBeNull;
		private readonly int _maxSize;

		public override bool Deserialize(object instance, ref object[] array, ref int i)
		{
			if (array[i++] is int len)
			{
				if (len > _maxSize || len < 0) return false;

				var desArray = new object[len];

				if (array.Length <= (i - 1) + len) return false;

				for (int desArrayIndex = 0; desArrayIndex < len; desArrayIndex++)
				{
					if (!(array[i] is T || (_canBeNull && array[i] == null)))
					{
						return false;
					}

					desArray[desArrayIndex] = array[i++];
				}

				_setValue(instance, desArray);

				return true;
			}

			return false;
		}

		public override void Serialize(object instance, ref object[] array, ref int i)
		{
			var arrayVal = (T[])_getValue(instance);
			array[i++] = arrayVal.Length;

			for (int arrayValInex = 0; arrayValInex < arrayVal.Length; arrayValInex++)
			{
				array[i++] = arrayVal[arrayValInex];
			}
		}

		public override void EstimateSize(object instance, ref int i)
			=> i += ((T[])_getValue(instance)).Length + 1;
	}
}