using Decorator.Compiler;
using Decorator.ModuleAPI;

namespace Decorator.Converter
{
	// [CLEAN]
	// TODO: Clean
	// ilSer and ilDes being passed as parameters is bad, should make an overload

	public class ConverterInstanceCreator : IConverterInstanceCreator
	{
		IConverter<T> IConverterInstanceCreator.Create<T>(BaseModule[] members, ILSerialize<T> ilSer, ILDeserialize<T> ilDes)
		{
			if (ilSer == null || ilDes == null)
			{
				return new Converter<T>(members);
			}
			else
			{
				return new ILConverter<T>(members, ilSer, ilDes);
			}
		}

		ICompiler<T> IConverterInstanceCreator.CreateCompiler<T>()
			=> new Compiler<T>();
	}
}