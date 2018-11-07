using Decorator.ModuleAPI;

using System;

namespace Decorator.ModuleAPI
{
	public class ModuleContainer
	{
		internal ModuleContainer(Type modifiedType, Member member, IConverterContainer container)
		{
			ModifiedType = modifiedType;
			Member = member;
			Container = container;
		}

		public Type ModifiedType { get; }
		public Member Member { get; }
		public IConverterContainer Container { get; }
	}
}