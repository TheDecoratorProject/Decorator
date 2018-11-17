using System;

namespace Decorator.ModuleAPI
{
	public interface IModuleBuilder
	{
		Type ModifyAppliedType(Type attributeAppliedTo);

		Module<T> Build<T>(ModuleContainer modContainer);
	}
}