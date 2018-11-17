using Decorator.Converter;

namespace Decorator
{
	internal static class StaticProvider
	{
		static StaticProvider() => Container = new ConverterContainer();

		public static ConverterContainer Container;
	}
}