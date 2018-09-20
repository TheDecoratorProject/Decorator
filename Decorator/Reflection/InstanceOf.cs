using System;
using System.Linq.Expressions;

namespace Decorator {

	internal static class InstanceOf<T> {

		public static T Create()
			=> Constructor();

		private static readonly Func<T> Constructor = Expression.Lambda<Func<T>>(
															Expression.New(typeof(T))
														).Compile();
	}
}