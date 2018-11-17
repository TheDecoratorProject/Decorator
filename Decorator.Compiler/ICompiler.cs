using Decorator.ModuleAPI;

using System;
using System.Reflection;

namespace Decorator.Compiler
{
	public interface ICompiler<T>
		where T : new()
	{
		BaseModule[] Compile(Func<MemberInfo, BaseContainer> getContainer);
	}
}