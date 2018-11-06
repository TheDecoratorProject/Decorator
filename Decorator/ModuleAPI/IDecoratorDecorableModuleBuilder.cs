using System;

namespace Decorator.ModuleAPI
{
	public interface IDecoratorDecorableModuleBuilder
	{
		DecoratorModule<T> BuildDecorable<T>(Type modifiedType, Member memberInfo)
			where T : IDecorable, new();
	}
}