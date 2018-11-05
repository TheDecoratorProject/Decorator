using Decorator.ModuleAPI;

using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class FlattenAttribute : Attribute, IDecoratorModuleBuilder, IDecoratorDecorableModuleBuilder
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public DecoratorModule<T> Build<T>(Type modifiedType, MemberInfo memberInfo) => EnsureIDecorable<FlattenAttribute>.InvokeBuild<T>(this, modifiedType, memberInfo);

		public DecoratorModule<T> BuildDecorable<T>(Type modifiedType, MemberInfo memberInfo)
			where T : IDecorable => new Module<T>(modifiedType, memberInfo);

		public class Module<T> : DecoratorModule<T>
			where T : IDecorable
		{
			public Module(Type modifiedType, MemberInfo memberInfo)
				: base(modifiedType, memberInfo)
			{
			}

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				if (!Converter<T>.TryDeserialize(array, ref i, out var result)) return false;
				SetValue(instance, result);
				return true;
			}

			public override void Serialize(object instance, ref object[] array, ref int i)
			{
				var data = Converter<T>.Serialize((T)GetValue(instance));

				for (var arrayIndex = 0; arrayIndex < data.Length; arrayIndex++)
				{
					array[i++] = data[arrayIndex];
				}
			}

			public override void EstimateSize(object instance, ref int i)
				=> i += DecoratorInfoContainer<T>.Members.EstimateSize((T)GetValue(instance));
		}
	}
}

/*using SwissILKnife;

using System;
using System.Linq;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class FlattenAttribute : Attribute, IDecoratorInfoAttribute
	{
		DecoratorInfo IDecoratorInfoAttribute.GetDecoratorInfo(MemberInfo memberValue)
		{
			var innerType = memberValue.GetMemberType();

			if(!innerType.GetInterfaces()
				.Contains(typeof(IDecorable)))
			{
				throw new InvalidDeclarationException();
			}

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

			for (var arrayIndex = 0; arrayIndex < data.Length; arrayIndex++)
			{
				array[i++] = data[arrayIndex];
			}
		}

		public override void EstimateSize(object instance, ref int i)
			=> i += DecoratorInfoContainer<T>.Members.EstimateSize((T)_getValue(instance));
	}
}*/