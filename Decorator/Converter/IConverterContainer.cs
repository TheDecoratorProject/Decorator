namespace Decorator
{
	public interface IConverterContainer
	{
		IConverter<T> Request<T>()
			where T : IDecorable, new();
	}
}