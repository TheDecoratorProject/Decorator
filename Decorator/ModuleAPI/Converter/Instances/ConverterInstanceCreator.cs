namespace Decorator.ModuleAPI
{
	public class ConverterInstanceCreator : IConverterInstanceCreator
	{
		IConverter<T> IConverterInstanceCreator.Create<T>(BaseModule[] members)
			=> new Converter<T>(members);

		ICompiler<T> IConverterInstanceCreator.CreateCompiler<T>()
			=> new Compiler<T>();
	}
}