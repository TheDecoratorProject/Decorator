using System;
using System.Reflection;

namespace Decorator.ModuleAPI
{
	public interface IDecoratorDecorableModuleBuilder
	{
		DecoratorModule<T> BuildDecorable<T>(Type modifiedType, MemberInfo memberInfo)
			where T : IDecorable;
	}
}