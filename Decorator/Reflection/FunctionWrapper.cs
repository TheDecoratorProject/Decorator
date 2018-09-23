using Decorator.Attributes;
using Decorator.Caching;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Decorator {

	internal class FunctionWrapper {

		public FunctionWrapper(MethodInfo method) {
			this.Method = method;
			this._versions = new ConcurrentHashcodeDictionary<Type, ILFunc>();
		}

		public MethodInfo Method;

		private ConcurrentHashcodeDictionary<Type, ILFunc> _versions;

		public ILFunc GetMethodFor(Type type) {
			if (this._versions.TryGetValue(type, out var res)) return res;

			var genMethod = this.Method
								.MakeGenericMethod(type);

			var method = genMethod
							.ILWrapRefSupport();

			this._versions.TryAdd(type, method);

			return method;
		}
	}
}