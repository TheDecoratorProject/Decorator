using System;
using System.Reflection;

namespace Decorator.ModuleAPI
{
	public interface IDecoratorDecorableModuleBuilder
	{
		DecoratorModule<T> BuildDecorable<T>(Type modifiedType, Member memberInfo)
			where T : IDecorable;
	}
}