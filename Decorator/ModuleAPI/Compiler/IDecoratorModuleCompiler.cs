namespace Decorator.ModuleAPI
{
	public interface IDecoratorModuleCompiler<T>
		where T : IDecorable, new()
	{
		BaseDecoratorModule[] Compile(IConverterContainer container);
	}
}