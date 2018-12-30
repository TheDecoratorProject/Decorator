using Decorator.ModuleAPI;
using System;

namespace Decorator.Converter
{
	public class ConverterContainerContainer : BaseContainer
	{
		public ConverterContainerContainer(Member member, IConverterContainer container)
		{
			Member = member;
			Container = container ?? throw new ArgumentNullException(nameof(container));
		}

		public IConverterContainer Container { get; }
		public override Member Member { get; }
	}
}