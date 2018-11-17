using System;
using System.Collections.Concurrent;

namespace Decorator.ModuleAPI
{
	public class ConverterContainer : IConverterContainer
	{
		public ConverterContainer() : this(new ConverterInstanceCreator())
		{
		}

		public ConverterContainer(IConverterInstanceCreator instantiator) => _instantiator = instantiator;

		private readonly IConverterInstanceCreator _instantiator;

		private readonly Cache _converters = new Cache();
		private readonly Cache _compilers = new Cache();

		public IConverter<T> RequestConverter<T>()
			where T : new()
			=> (IConverter<T>)_converters.Request(() => _instantiator.Create<T>(RequestCompiler<T>().Compile(this)));

		public ICompiler<T> RequestCompiler<T>()
			where T : new()
			=> (ICompiler<T>)_compilers.Request(() => _instantiator.CreateCompiler<T>());
	}
}