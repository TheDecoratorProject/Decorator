using Decorator.ModuleAPI;

namespace Decorator.Converter
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