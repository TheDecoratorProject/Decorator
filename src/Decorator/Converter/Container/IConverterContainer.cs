using Decorator.Compiler;

namespace Decorator.Converter
{
	public interface IConverterContainer
	{
		IConverter<T> RequestConverter<T>()
			where T : new();

		ICompiler<T> RequestCompiler<T>()
			where T : new();
	}
}