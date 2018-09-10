using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator {

	public class MethodDeserializerManager<TClass>
		where TClass : class {

		public MethodDeserializerManager() {
			this.Cache = new Cache<Type, MethodInfo[]>();

			this.MethodInfoCache = new Cache<MethodInfo, Func<object, object[], object>>();

			//TODO: clean
			// with like a reflection helper class
			// or something

			this._cast = this.GetType().GetMethod(nameof(CastObj), BindingFlags.NonPublic | BindingFlags.Static);

			var dict = new Dictionary<Type, List<MethodInfo>>();

			foreach (var i in typeof(TClass).GetMethods().Where(x => x.HasAttribute<Attributes.DeserializedHandlerAttribute>(out var _)))
				if (dict.TryGetValue(i.GetParameters()[0].ParameterType, out var val))
					val.Add(i);
				else dict[i.GetParameters()[0].ParameterType] = new List<MethodInfo> { i };

			foreach (var i in dict)
				this.Cache.Retrieve(i.Key, () => i.Value.ToArray());
		}

		private MethodInfo _cast;

		public Cache<Type, MethodInfo[]> Cache { get; }

		public Cache<MethodInfo, Func<object, object[], object>> MethodInfoCache { get; }

		public MethodInfo[] GetHandlers() => this.Cache.SelectMany(x => x.Value).ToArray();

		public MethodInfo[] GetHandlersFor<TItem>()
			=> GetHandlersFor(typeof(TItem));

		public MethodInfo[] GetHandlersFor(Type item)
			=> this.Cache.Retrieve(item, () => {
				throw new Exceptions.DecoratorException("Type doesn't exist.");
			});

		public void InvokeMethod<TItem>(MethodInfo method, TClass instance, TItem item)
			=> InvokeMethod(method, (object)instance, item);

		public void InvokeMethod<TItem>(MethodInfo method, object instance, TItem item) {
			//method.Invoke(instance, new object[] { item });

			var d = this.MethodInfoCache.Retrieve(method, () =>
				IL.Wrap(method));

			d(instance, new object[] { item });

			//Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(item?.GetType()), instance, method)
			//	.DynamicInvoke(item);

			//method.Invoke(instance, new object[] { item });
		}

		private static T CastObj<T>(object input)
			=> (T)input;
	}
}