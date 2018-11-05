using System;
using System.Reflection;

namespace Decorator.ModuleAPI
{
	public interface IOnlyDecorablesDecorableModuleBuilder
	{
		DecoratorModule<T> BuildDecorable<T>(Type modifiedType, MemberInfo memberInfo)
			where T : IDecorable;
	}
}