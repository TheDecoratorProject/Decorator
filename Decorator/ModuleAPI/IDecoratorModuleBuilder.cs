
using System;
using System.Reflection;

namespace Decorator.ModuleAPI
{
	public interface IDecoratorModuleBuilder
	{
		Type ModifyAppliedType(Type attributeAppliedTo);
		DecoratorModule<T> Build<T>(Type modifiedType, MemberInfo memberInfo);
	}
}