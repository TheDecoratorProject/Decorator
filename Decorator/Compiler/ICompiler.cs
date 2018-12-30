namespace Decorator
{
	public interface ICompiler<T>
	{
		IDecoration[] Compile(IDiscovery<T> discovery, IDecorationFactoryBuilder builder);
	}
}