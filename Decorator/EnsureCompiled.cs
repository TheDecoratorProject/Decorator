namespace Decorator
{
	public static class EnsureCompiled<T>
		where T : IDecorable
	{
		public static void Ensure() => DecoratorInfoCompiler<T>.Compile();
	}
}