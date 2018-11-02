using SwissILKnife;

using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class OptionalAttribute : Attribute, IDecoratorInfoAttribute
	{
		DecoratorInfo IDecoratorInfoAttribute.GetDecoratorInfo(MemberInfo memberValue)
		{
			return Make.Class(typeof(Optional<>)).Generic(memberValue.GetMemberType())
					.CreateDecoratorInfo(memberValue);
		}
	}

	internal class Optional<T> : DecoratorInfo
	{
		public Optional(MemberInfo memberInfo)
		{
			_getValue = MemberUtils.GetGetMethod(memberInfo);
			_setValue = MemberUtils.GetSetMethod(memberInfo);
		}

		protected Func<object, object> _getValue;
		protected Action<object, object> _setValue;

		public override bool Deserialize(object instance, ref object[] array, ref int i)
		{
			if (array[i++] is T)
			{
				_setValue(instance, array[i - 1]);
			}

			return true;
		}

		public override void Serialize(object instance, ref object[] array, ref int i) => array[i++] = _getValue(instance);

		public override void EstimateSize(object instance, ref int i) => i++;
	}
}