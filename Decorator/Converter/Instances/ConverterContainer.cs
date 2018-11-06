﻿using System;
using System.Collections.Concurrent;

namespace Decorator
{
	public class ConverterContainer : IConverterContainer
	{
		public ConverterContainer() : this(new ConverterInstanceCreator())
		{
		}

		public ConverterContainer(IConverterInstanceCreator instantiator)
		{
			_instantiator = instantiator;
			_converterStorage = new ConcurrentDictionary<Type, IConverter>();
		}

		private readonly IConverterInstanceCreator _instantiator;
		private readonly ConcurrentDictionary<Type, IConverter> _converterStorage;

		public IConverter<T> Request<T>()
			where T : IDecorable, new()
		{
			if (_converterStorage.TryGetValue(typeof(T), out var result))
			{
				return (IConverter<T>)result;
			}

			result = _instantiator.Create(_instantiator.CreateCompiler<T>(), this);

			_converterStorage.TryAdd(typeof(T), result);

			return (IConverter<T>)result;
		}
	}
}