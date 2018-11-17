using SwissILKnife;

using System;

namespace Decorator.ModuleAPI
{
	public abstract class Module<T> : BaseModule
	{
		protected Module(BaseContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			var actualMember = container.Member.GetMember;

			_setVal = MemberUtils.GetSetMethod(actualMember);
			_getVal = MemberUtils.GetGetMethod(actualMember);

			ModuleContainer = container;
		}

		public sealed override BaseContainer ModuleContainer { get; }

		private readonly Action<object, object> _setVal;
		private readonly Func<object, object> _getVal;

		protected void SetValue(object instance, object value)
			=> _setVal(instance, value);

		protected object GetValue(object instance)
			=> _getVal(instance);
	}
}