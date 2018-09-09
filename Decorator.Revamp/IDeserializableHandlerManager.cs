using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Decorator {
	public interface IDeserializableHandlerManager<TClass>
		where TClass : class {
		ICache<Type, MethodInfo[]> Cache { get; }

		MethodInfo[] GetHandlers();

		TItem[] GetItemTypes<TItem>();

		MethodInfo[] GetHandlersFor<TItem>();
		MethodInfo[] GetHandlersFor(Type item);

		void InvokeMethod<TItem>(MethodInfo method, TClass instance, TItem item);
		void InvokeMethod(MethodInfo method, object instance, object item);
	}

	public class DeserializableHandlerManager<TClass> : IDeserializableHandlerManager<TClass>
		where TClass : class {
		public DeserializableHandlerManager() {
			this.Cache = new CacheManager<Type, MethodInfo[]>();

			//TODO: clean
			// with like a reflection helper class
			// or something

			var dict = new Dictionary<Type, List<MethodInfo>>();

			foreach (var i in typeof(TClass).GetMethods())
				if (i.GetCustomAttributes(typeof(Attributes.DeserializedHandlerAttribute), true).Length > 0)
					if (dict.TryGetValue(i.GetParameters()[0].ParameterType, out var val))
						val.Add(i);
					else dict[i.GetParameters()[0].ParameterType] = new List<MethodInfo> {
						i
					};

			foreach (var i in dict)
				this.Cache.Store(i.Key, i.Value.ToArray());
		}

		public ICache<Type, MethodInfo[]> Cache { get; }

		public MethodInfo[] GetHandlers() => this.Cache.SelectMany(x => x.Value).ToArray();

		public MethodInfo[] GetHandlersFor<TItem>()
			=> GetHandlersFor(typeof(TItem));

		public MethodInfo[] GetHandlersFor(Type item)
			=> this.Cache.Retrieve(item, () => {
				throw new Exceptions.DecoratorException("Type doesn't exist.");
			});

		public TItem[] GetItemTypes<TItem>() => throw new NotImplementedException();

		public void InvokeMethod<TItem>(MethodInfo method, TClass instance, TItem item)
			=> InvokeMethod(method, (object)instance, (object)item);

		public void InvokeMethod(MethodInfo method, object instance, object item) {
			method.Invoke(instance, new object[] { item });
		}
	}
}