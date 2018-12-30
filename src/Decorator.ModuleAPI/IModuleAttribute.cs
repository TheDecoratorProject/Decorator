using System;

namespace Decorator.ModuleAPI
{
	public interface IModuleAttribute
	{
		Type ModifyAppliedType(Type attributeAppliedTo);

		Module<T> Build<T>(BaseContainer modContainer);
	}
}