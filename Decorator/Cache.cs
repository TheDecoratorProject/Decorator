using System;
using System.Collections.Concurrent;

namespace Decorator.ModuleAPI
{
	internal class Cache
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