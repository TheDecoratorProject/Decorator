using Decorator.ModuleAPI;

using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class OptionalAttribute : Attribute, IDecoratorModuleBuilder
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public DecoratorModule<T> Build<T>(Type modifiedType, MemberInfo memberInfo)
			=> new Module<T>(modifiedType, memberInfo);

		public class Module<T> : DecoratorModule<T>
		{
			public Module(Type modifiedType, MemberInfo memberInfo)
				: base(modifiedType, memberInfo)
			{
				if (!modifiedType.IsValueType)
				{
					_canBeNull = true;
				}
			}

			private readonly bool _canBeNull;

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				var iBeforeInc = i;

				if (array[i++] is T ||
					(_canBeNull &&
					array[iBeforeInc] == null))
				{
					SetValue(instance, array[iBeforeInc]);
				}

				return true;
			}

			public override void Serialize(object instance, ref object[] array, ref int i) => array[i++] = GetValue(instance);

			public override void EstimateSize(object instance, ref int i) => i++;
		}
	}
}

/*using SwissILKnife;

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
			_canBeNull = !memberInfo.GetMemberType().IsValueType;
		}

		protected Func<object, object> _getValue;
		protected Action<object, object> _setValue;
		private readonly bool _canBeNull;

		public override bool Deserialize(object instance, ref object[] array, ref int i)
		{
			var iBeforeInc = i;

			if (array[i++] is T ||
				(_canBeNull &&
				array[iBeforeInc] == null))
			{
				_setValue(instance, array[iBeforeInc]);
			}

			return true;
		}

		public override void Serialize(object instance, ref object[] array, ref int i) => array[i++] = _getValue(instance);

		public override void EstimateSize(object instance, ref int i) => i++;
	}
}*/