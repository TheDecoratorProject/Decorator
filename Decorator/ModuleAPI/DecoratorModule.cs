using SwissILKnife;

using System;
using System.Reflection;

namespace Decorator.ModuleAPI
{
	public abstract class DecoratorModule<T> : BaseDecoratorModule
	{
		protected DecoratorModule(Type modifiedType, MemberInfo memberInfo)
		{
			_setVal = MemberUtils.GetSetMethod(memberInfo);
			_getVal = MemberUtils.GetGetMethod(memberInfo);
		}

		private Action<object, object> _setVal;
		private Func<object, object> _getVal;

		protected void SetValue(object instance, object value)
			=> _setVal(instance, value);

		protected object GetValue(object instance)
			=> _getVal(instance);
	}
}