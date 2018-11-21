using Decorator.Compiler;
using Decorator.ModuleAPI;

namespace Decorator.Converter
{
	public interface IConverterInstanceCreator
	{
		IConverter<T> Create<T>(BaseModule[] members, ILSerialize<T> ilSer = null, ILDeserialize<T> ilDes = null)
			where T : new();

		ICompiler<T> CreateCompiler<T>()
			where T : new();
	}
}