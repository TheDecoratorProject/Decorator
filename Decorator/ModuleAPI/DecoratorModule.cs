using SwissILKnife;

using System;

namespace Decorator.ModuleAPI
{
	public abstract class DecoratorModule<T> : BaseDecoratorModule
	{
		protected DecoratorModule(ModuleContainer container)
		{
			var actualMember = container.Member.GetMember;

			_setVal = MemberUtils.GetSetMethod(actualMember);
			_getVal = MemberUtils.GetGetMethod(actualMember);

			OriginalType = container.Member.GetMemberType();
			ModifiedType = container.ModifiedType;
			Member = container.Member;
			Container = container.Container;
		}

		public sealed override Type ModifiedType { get; }
		public sealed override Type OriginalType { get; }
		public sealed override Member Member { get; }
		public sealed override IConverterContainer Container { get; }

		private readonly Action<object, object> _setVal;
		private readonly Func<object, object> _getVal;

		protected void SetValue(object instance, object value)
			=> _setVal(instance, value);

		protected object GetValue(object instance)
			=> _getVal(instance);
	}
}