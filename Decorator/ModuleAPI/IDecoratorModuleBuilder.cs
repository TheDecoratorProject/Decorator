using System;

namespace Decorator.ModuleAPI
{
	public interface IDecoratorModuleBuilder
	{
		Type ModifyAppliedType(Type attributeAppliedTo);

		DecoratorModule<T> Build<T>(ModuleContainer modContainer);
	}
}