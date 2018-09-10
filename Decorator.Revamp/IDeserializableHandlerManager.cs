using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator {

	public interface IDeserializableHandlerManager<TClass>
		where TClass : class {
		ICache<Type, MethodInfo[]> Cache { get; }

		MethodInfo[] GetHandlers();

		TItem[] GetItemTypes<TItem>();

		MethodInfo[] GetHandlersFor<TItem>();

		MethodInfo[] GetHandlersFor(Type item);

		void InvokeMethod<TItem>(MethodInfo method, TClass instance, TItem item);

		void InvokeMethod<TItem>(MethodInfo method, object instance, TItem item);
	}

	public class DeserializableHandlerManager<TClass> : IDeserializableHandlerManager<TClass>
		where TClass : class {

		public DeserializableHandlerManager() {
			this.Cache = new CacheManager<Type, MethodInfo[]>();

			this.MethodInfoCache = new CacheManager<MethodInfo, Func<object, object[], object>>();

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

		public ICache<Type, MethodInfo[]> Cache { get; }

		public ICache<MethodInfo, Func<object, object[], object>> MethodInfoCache { get; }

		public MethodInfo[] GetHandlers() => this.Cache.SelectMany(x => x.Value).ToArray();

		public MethodInfo[] GetHandlersFor<TItem>()
			=> GetHandlersFor(typeof(TItem));

		public MethodInfo[] GetHandlersFor(Type item)
			=> this.Cache.Retrieve(item, () => {
				throw new Exceptions.DecoratorException("Type doesn't exist.");
			});

		public TItem[] GetItemTypes<TItem>() => throw new NotImplementedException();

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