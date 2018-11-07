using System;

namespace Decorator.ModuleAPI
{
	public interface IDecoratorModuleBuilder
	{
		Type ModifyAppliedType(Type attributeAppliedTo);

		DecoratorModule<T> Build<T>(ModuleContainer modContainer);
	}

	public interface IDecoratorDecorableModuleBuilder
	{
		DecoratorModule<T> BuildDecorable<T>(ModuleContainer modContainer)
			where T : IDecorable, new();
	}
}