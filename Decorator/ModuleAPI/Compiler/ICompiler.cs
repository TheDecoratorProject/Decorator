namespace Decorator.ModuleAPI
{
	public interface ICompiler<T>
		where T : IDecorable, new()
	{
		BaseModule[] Compile(IConverterContainer container);
	}
}