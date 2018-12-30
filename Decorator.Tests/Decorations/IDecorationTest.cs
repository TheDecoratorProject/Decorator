namespace Decorator.Tests
{
	public abstract class IDecorationTest<T>
		where T : IDecorationFactory, new()
	{
		public T GetInstance() => new T();
	}
}