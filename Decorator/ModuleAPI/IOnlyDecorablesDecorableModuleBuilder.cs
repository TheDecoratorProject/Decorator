using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Decorator.ModuleAPI
{
	public interface IOnlyDecorablesDecorableModuleBuilder
	{
		DecoratorModule<T> BuildDecorable<T>(Type modifiedType, MemberInfo memberInfo)
			where T : IDecorable;
	}
}
