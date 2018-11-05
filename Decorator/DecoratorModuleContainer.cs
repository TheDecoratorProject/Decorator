using Decorator.ModuleAPI;
using System.Collections.ObjectModel;

namespace Decorator
{
	public static class DecoratorModuleContainer<T>
	{
		static DecoratorModuleContainer() => MembersValue = DecoratorModuleCompiler<T>.Compile();

		public static ReadOnlyCollection<BaseDecoratorModule> Members
			=> new ReadOnlyCollection<BaseDecoratorModule>(MembersValue);

		internal static BaseDecoratorModule[] MembersValue;
	}
}