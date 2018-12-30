using Decorator.Compiler;

namespace Decorator.Converter
{
	public interface IILConverter<T> : IConverter<T>
		where T : new()
	{
		ILDeserialize<T> Deserialize { get; }
		ILSerialize<T> Serialize { get; }
	}
}