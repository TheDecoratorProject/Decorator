using Decorator.ModuleAPI;
using System;

namespace Decorator
{
	public class ModuleContainer : IModuleContainer
	{
		internal ModuleContainer(Type originalType, Type modifiedType, Member member, IConverterContainer container)
		{
			OriginalType = originalType;
			ModifiedType = modifiedType;
			Member = member;
			Container = container;
		}

		public Type OriginalType { get; }
		public Type ModifiedType { get; }
		public Member Member { get; }
		public IConverterContainer Container { get; }
	}
}
