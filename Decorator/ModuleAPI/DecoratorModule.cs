using SwissILKnife;

using System;

namespace Decorator.ModuleAPI
{
	public abstract class DecoratorModule<T> : BaseDecoratorModule
	{
		protected DecoratorModule(Type modifiedType, Member member)
		{
			var actualMember = member.GetMember;

			_setVal = MemberUtils.GetSetMethod(actualMember);
			_getVal = MemberUtils.GetGetMethod(actualMember);

			OriginalType = member.GetMemberType();
			ModifiedType = modifiedType;
			Member = member;
		}

		public sealed override Type ModifiedType { get; }
		public sealed override Type OriginalType { get; }
		public sealed override Member Member { get; }

		private readonly Action<object, object> _setVal;
		private readonly Func<object, object> _getVal;

		protected void SetValue(object instance, object value)
			=> _setVal(instance, value);

		protected object GetValue(object instance)
			=> _getVal(instance);
	}
}