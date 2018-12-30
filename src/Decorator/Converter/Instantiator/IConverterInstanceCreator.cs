using Decorator.Compiler;
using Decorator.ModuleAPI;

namespace Decorator.Converter
{
	// [CLEAN]
	// TODO: Clean
	// passing these as parameters, should make overload or something (new interface?)

	public interface IConverterInstanceCreator
	{
		IConverter<T> Create<T>(BaseModule[] members, ILSerialize<T> ilSer = null, ILDeserialize<T> ilDes = null)
			where T : new();

		ICompiler<T> CreateCompiler<T>()
			where T : new();
	}
}