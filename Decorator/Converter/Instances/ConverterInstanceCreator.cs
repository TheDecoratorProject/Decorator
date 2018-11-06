namespace Decorator
{
	public class ConverterInstanceCreator : IConverterInstanceCreator
	{
		IConverter<T> IConverterInstanceCreator.Create<T>(IDecoratorModuleCompiler<T> compiler, IConverterContainer container)
			=> new Converter<T>(compiler.Compile(container));

		IDecoratorModuleCompiler<T> IConverterInstanceCreator.CreateCompiler<T>()
			=> new DecoratorModuleCompiler<T>();
	}
}