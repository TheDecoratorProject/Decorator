namespace Decorator.ModuleAPI
{
	public interface IConverterInstanceCreator
	{
		IConverter<T> Create<T>(BaseDecoratorModule[] members)
			where T : IDecorable, new();

		IDecoratorModuleCompiler<T> CreateCompiler<T>()
			where T : IDecorable, new();
	}
}