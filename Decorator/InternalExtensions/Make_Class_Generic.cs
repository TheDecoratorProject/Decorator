using System;
using System.Collections.Concurrent;

namespace Decorator
{
	// Sample usage of this class: Make.Class(typeof(Array<>)).GenericType(typeof(type));
	internal static class Make
	{
		public interface IGenericMaker
		{
			Type Generic(Type genericType);
		}

		private class GenericTypeCacher : IGenericMaker
		{
			public GenericTypeCacher(Type classType)
				=> _classType = classType;

			private readonly Type _classType;
			private readonly ConcurrentDictionary<Type, Type> _cache = new ConcurrentDictionary<Type, Type>();

			public Type Generic(Type genericType)
			{
				if (_cache.TryGetValue(genericType, out var result)) return result;

				result = _classType.MakeGenericType(genericType);

				_cache.TryAdd(genericType, result);

				return result;
			}
		}

		private static ConcurrentDictionary<Type, IGenericMaker> _genericClassDictionary = new ConcurrentDictionary<Type, IGenericMaker>();

		public static IGenericMaker Class(Type classType)
		{
			if (_genericClassDictionary.TryGetValue(classType, out var result)) return result;

			result = new GenericTypeCacher(classType);

			_genericClassDictionary.TryAdd(classType, result);

			return result;
		}
	}
}