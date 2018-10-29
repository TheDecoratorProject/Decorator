using SwissILKnife;
using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class RequiredAttribute : Attribute, IDecoratorInfoAttribute
	{
		DecoratorInfo IDecoratorInfoAttribute.GetDecoratorInfo(MemberInfo memberValue)
		{
			return Make.Class(typeof(Required<>)).Generic(memberValue.GetMemberType())
					.CreateDecoratorInfo(memberValue);
		}
	}

	internal class Required<T> : DecoratorInfo
	{
		public Required(MemberInfo memberInfo)
		{
			_getValue = MemberUtils.GetGetMethod(memberInfo);
			_setValue = MemberUtils.GetSetMethod(memberInfo);
		}

		protected Func<object, object> _getValue;
		protected Action<object, object> _setValue;

		public override bool Deserialize(object instance, ref object[] array, ref int i)
		{
			if (array[i] is T)
			{
				_setValue(instance, array[i++]);
				return true;
			}

			return false;
		}

		public override void Serialize(object instance, ref object[] array, ref int i) => array[i++] = _getValue(instance);

		public override void EstimateSize(object instance, ref int i) => i++;
	}
}