using SwissILKnife;
using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class FlattenAttribute : Attribute, IDecoratorInfoAttribute
	{
		DecoratorInfo IDecoratorInfoAttribute.GetDecoratorInfo(MemberInfo memberValue)
		{
			return Make.Class(typeof(Flatten<>)).Generic(memberValue.GetMemberType())
					.CreateDecoratorInfo(memberValue);
		}
	}

	internal class Flatten<T> : DecoratorInfo
		where T : IDecorable
	{
		public Flatten(MemberInfo memberInfo)
		{
			_getValue = MemberUtils.GetGetMethod(memberInfo);
			_setValue = MemberUtils.GetSetMethod(memberInfo);
		}

		protected Func<object, object> _getValue;
		protected Action<object, object> _setValue;

		public override bool Deserialize(object instance, ref object[] array, ref int i)
		{
			if (!Converter<T>.TryDeserialize(array, ref i, out var result)) return false;
			_setValue(instance, result);
			return true;
		}

		public override void Serialize(object instance, ref object[] array, ref int i)
		{
			var data = Converter<T>.Serialize((T)_getValue(instance));

			for (int arrayIndex = 0; arrayIndex < data.Length; arrayIndex++)
			{
				array[i++] = data[arrayIndex];
			}
		}

		public override void EstimateSize(object instance, ref int i)
			=> i += DecoratorInfoCompiler<T>.Members.EstimateSize((T)_getValue(instance));
	}
}