namespace Decorator
{
	public interface IConverterInstanceCreator
	{
		IConverter<T> Create<T>(IDecoratorModuleCompiler<T> compiler, IConverterContainer container)
			where T : IDecorable, new();

		IDecoratorModuleCompiler<T> CreateCompiler<T>()
			where T : IDecorable, new();
	}
}