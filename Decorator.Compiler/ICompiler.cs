using System;
using System.Reflection;

namespace Decorator.ModuleAPI
{
	public interface ICompiler<T>
		where T : new()
	{
		BaseModule[] Compile(Func<MemberInfo, BaseContainer> getContainer);
	}
}