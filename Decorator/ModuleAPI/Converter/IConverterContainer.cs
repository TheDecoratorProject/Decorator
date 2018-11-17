namespace Decorator.ModuleAPI
{
	public interface IConverterContainer
	{
		IConverter<T> RequestConverter<T>()
			where T : IDecorable, new();

		ICompiler<T> RequestCompiler<T>()
			where T : IDecorable, new();
	}
}