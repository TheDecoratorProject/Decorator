using System;
using System.Linq.Expressions;

namespace Decorator {

	public static class InstanceOf<T> {

		public static T Create()
			=> Constructor();

		public static Func<T> Constructor = Expression.Lambda<Func<T>>(
												Expression.New(typeof(T))
											).Compile();
	}
}