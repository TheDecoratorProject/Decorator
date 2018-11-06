namespace Decorator
{
	public static class EnsureCompiled<T>
		where T : IDecorable, new()
	{
		public static void Ensure() => DecoratorModuleCompiler<T>.Compile();
	}
}