using Decorator.Compiler;
using Decorator.ModuleAPI;

namespace Decorator.Converter
{
	public class ConverterInstanceCreator : IConverterInstanceCreator
	{
		IConverter<T> IConverterInstanceCreator.Create<T>(BaseModule[] members)
			=> new Converter<T>(members);

		ICompiler<T> IConverterInstanceCreator.CreateCompiler<T>()
			=> new Compiler<T>();
	}
}