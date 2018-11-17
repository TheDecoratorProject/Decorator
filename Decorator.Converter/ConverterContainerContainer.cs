using Decorator.ModuleAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
	public class ConverterContainerContainer : BaseContainer
	{
		internal ConverterContainerContainer(Member member, IConverterContainer container)
		{
			Member = member;
			Container = container;
		}

		public IConverterContainer Container { get; }
		public override Member Member { get; }
	}
}
