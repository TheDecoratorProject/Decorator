namespace Decorator.ModuleAPI
{
	public interface IConverterInstanceCreator
	{
		IConverter<T> Create<T>(BaseModule[] members)
			where T : IDecorable, new();

		ICompiler<T> CreateCompiler<T>()
			where T : IDecorable, new();
	}
}