using Decorator.Attributes;
using System;
using System.Linq;

namespace Decorator {
	public interface ITypeManager {
		ICache<Type, IManagedType[]> Cache { get; }

		void RegisterType<T>();
		void RegisterType(Type t);

		bool QualifiesAsType<T>(Message m);
		bool QualifiesAsType(Type t, Message m);

		T DeserializeToType<T>(Message m) where T : new();
		object DeserializeToType(Type t, Message m);
	}

	public class TypeManager : ITypeManager {
		public TypeManager() {
			this.Cache = new CacheManager<Type, IManagedType[]>();
		}

		public ICache<Type, IManagedType[]> Cache { get; }

		public T DeserializeToType<T>(Message m)
			where T : new()
			=> (T)DeserializeToType(typeof(T), m);

		public object DeserializeToType(Type t, Message m) {
			// we assume that QualifiesAsType has already been called

			var instance = Activator.CreateInstance(t);

			var positions = t.GetPositions();

			foreach(var i in positions) {
				i.SetValue(instance, m?.Arguments[i.GetPosition()]);
			}

			return instance;
		}

		public bool QualifiesAsType<T>(Message m)
			=> QualifiesAsType(typeof(T), m);

		public bool QualifiesAsType(Type t, Message m) {
			if (!t.HasAttribute<MessageAttribute>(out var attrib) ||
				attrib.Type != m.Type) return false;

			var positions = t.GetPositions();

			var maxPos = positions.Length > 0
				? positions
					.Max(x => x.GetPosition()) + 1
				: 0;

			if (m.Count != maxPos) return false;

			foreach(var i in positions)
				if (i.PropertyType != m?.Arguments[i.GetPosition()]?.GetType())
					return false;

			return true;
		}

		public void RegisterType<T>()
			=> RegisterType(typeof(T));

		public void RegisterType(Type t) {
			
		}
	}
}