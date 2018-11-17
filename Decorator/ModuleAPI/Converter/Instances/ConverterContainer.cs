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
			where T : IDecorable, new()
			=> (IConverter<T>)_converters.Request(() => _instantiator.Create<T>(RequestCompiler<T>().Compile(this)));

		public ICompiler<T> RequestCompiler<T>()
			where T : IDecorable, new()
			=> (ICompiler<T>)_compilers.Request(() => _instantiator.CreateCompiler<T>());
	}

	public class Cache
	{
		private readonly ConcurrentDictionary<Type, object> _storage
			= new ConcurrentDictionary<Type, object>();

		public object Request<TType>(Func<TType> create)
		{
			if(_storage.TryGetValue(typeof(TType), out var result))
			{
				return result;
			}

			result = create();

			_storage.TryAdd(typeof(TType), result);

			return result;
		}
	}
}