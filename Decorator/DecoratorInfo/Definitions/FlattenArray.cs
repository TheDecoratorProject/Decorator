using SwissILKnife;
using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class FlattenArrayAttribute : Attribute, IDecoratorInfoAttribute
	{
		DecoratorInfo IDecoratorInfoAttribute.GetDecoratorInfo(MemberInfo memberValue)
		{
			return Make.Class(typeof(FlattenArray<>)).Generic(memberValue.GetMemberType().GetElementType())
					.CreateDecoratorInfo(memberValue);
		}
	}

	internal class FlattenArray<T> : DecoratorInfo
		where T : IDecorable
	{
		public FlattenArray(MemberInfo memberInfo)
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
					if (!Converter<T>.TryDeserialize(array, ref i, out var item)) return false;

					desArray[desArrayIndex] = item;
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

			for (int arrayValIndex = 0; arrayValIndex < arrayVal.Length; arrayValIndex++)
			{
				var data = Converter<T>.Serialize(arrayVal[arrayValIndex]);

				for (int arrayIndex = 0; arrayIndex < data.Length; arrayIndex++)
				{
					array[i++] = data[arrayIndex];
				}
			}
		}

		public override void EstimateSize(object instance, ref int i)
		{
			var array = (T[])_getValue(instance);

			i++;

			for (int arrayIndex = 0; arrayIndex < array.Length; arrayIndex++)
			{
				i += DecoratorInfoCompiler<T>.Members.EstimateSize(array[arrayIndex]);
			}
		}
	}
}