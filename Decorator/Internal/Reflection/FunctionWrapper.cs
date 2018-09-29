﻿using Decorator.Caching;

using System;
using System.Reflection;

namespace Decorator
{
	internal class FunctionWrapper
	{
		public FunctionWrapper(MethodInfo method)
		{
			Method = method;
			_versions = new ConcurrentHashcodeDictionary<Type, ILFunc>();
		}

		public MethodInfo Method;

		private readonly ConcurrentHashcodeDictionary<Type, ILFunc> _versions;

		public ILFunc GetMethodFor(Type type)
		{
			if (_versions.TryGetValue(type, out var res)) return res;

			var genMethod = Method
								.MakeGenericMethod(type);

			var method = genMethod
							.ILWrapRefSupport();

			_versions.TryAdd(type, method);

			return method;
		}
	}
}