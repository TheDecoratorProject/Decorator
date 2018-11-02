using SwissILKnife;

using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ArrayAttribute : Attribute, IDecoratorInfoAttribute
	{
		DecoratorInfo IDecoratorInfoAttribute.GetDecoratorInfo(MemberInfo memberValue)
		{
			return Make.Class(typeof(Array<>)).Generic(memberValue.GetMemberType().GetElementType())
					.CreateDecoratorInfo(memberValue);
		}
	}

	internal class Array<T> : DecoratorInfo
	{
		public Array(MemberInfo memberInfo)
		{
			_getValue = MemberUtils.GetGetMethod(memberInfo);
			_setValue = MemberUtils.GetSetMethod(memberInfo);
		}

		protected Func<object, object> _getValue;
		protected Action<object, object> _setValue;

		public override bool Deserialize(object instance, ref object[] array, ref int i)
		{
			if (array[i++] is int len)
			{
				var desArray = new object[len];

				for (int desArrayIndex = 0; desArrayIndex < len; desArrayIndex++)
				{
					if (array.Length <= i ||
						!(array[i] is T)) return false;

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