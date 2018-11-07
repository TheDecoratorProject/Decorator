using System;
using System.Collections.Concurrent;

namespace Decorator.ModuleAPI
{
	public class ConverterContainer : IConverterContainer
	{
		public ConverterContainer() : this(new ConverterInstanceCreator())
		{
		}

		public ConverterContainer(IConverterInstanceCreator instantiator)
		{
			_instantiator = instantiator;
		}

		private readonly IConverterInstanceCreator _instantiator;

		private readonly ConcurrentDictionary<Type, object> _converterStorage
			= new ConcurrentDictionary<Type, object>();

		private readonly ConcurrentDictionary<Type, object> _compilerStorage
			= new ConcurrentDictionary<Type, object>();

		public IConverter<T> Request<T>()
			where T : IDecorable, new()
		{
			if (_converterStorage.TryGetValue(typeof(T), out var result))
			{
				return (IConverter<T>)result;
			}

			result = _instantiator.Create<T>(RequestCompiler<T>().Compile(this));

			_converterStorage.TryAdd(typeof(T), result);

			return (IConverter<T>)result;
		}

		public IDecoratorModuleCompiler<T> RequestCompiler<T>()
			where T : IDecorable, new()
		{
			if (_compilerStorage.TryGetValue(typeof(T), out var result))
			{
				return (IDecoratorModuleCompiler<T>)result;
			}

			result = _instantiator.CreateCompiler<T>();

			_compilerStorage.TryAdd(typeof(T), result);

			return (IDecoratorModuleCompiler<T>)result;
		}
	}
}