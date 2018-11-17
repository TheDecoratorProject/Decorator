namespace Decorator.ModuleAPI
{
	public interface ICompiler<T>
		where T : new()
	{
		BaseModule[] Compile(IConverterContainer container);
	}
}