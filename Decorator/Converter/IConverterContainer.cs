namespace Decorator
{
	public interface IConverterContainer
	{
		IConverter<T> Request<T>()
			where T : IDecorable, new();

		IDecoratorModuleCompiler<T> RequestCompiler<T>()
			where T : IDecorable, new();
	}
}