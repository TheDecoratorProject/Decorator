namespace Decorator.ModuleAPI
{
	public interface IConverterInstanceCreator
	{
		IConverter<T> Create<T>(BaseModule[] members)
			where T : new();

		ICompiler<T> CreateCompiler<T>()
			where T : new();
	}
}