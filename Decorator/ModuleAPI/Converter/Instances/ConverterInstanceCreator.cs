namespace Decorator.ModuleAPI
{
	public class ConverterInstanceCreator : IConverterInstanceCreator
	{
		IConverter<T> IConverterInstanceCreator.Create<T>(BaseDecoratorModule[] members)
			=> new Converter<T>(members);

		IDecoratorModuleCompiler<T> IConverterInstanceCreator.CreateCompiler<T>()
			=> new DecoratorModuleCompiler<T>();
	}
}