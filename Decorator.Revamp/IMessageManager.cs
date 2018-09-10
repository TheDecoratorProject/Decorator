using Decorator.Attributes;

using System;
using System.Collections.Generic;

namespace Decorator {

	public interface IMessageManager {

		bool QualifiesAsType<T>(BaseMessage m);

		bool QualifiesAsType(Type t, BaseMessage m);

		T DeserializeToType<T>(BaseMessage m) where T : new();

		object DeserializeToType(Type t, BaseMessage m);

		bool QualifiesAsRepeatableType<T>(BaseMessage m);

		bool QualifiesAsRepeatableType(Type t, BaseMessage m);

		IEnumerable<T> DeserializeRepeatableToType<T>(BaseMessage m) where T : new();

		IEnumerable<object> DeserializeRepeatableToType(Type t, BaseMessage m);
	}

	public class MessageManager : IMessageManager {

		public MessageManager() {
			this.Cache = new CacheManager<Type, IMessageDefinition>();
		}

		public ICache<Type, IMessageDefinition> Cache { get; }

		public IMessageDefinition GetDefinitionFor<T>()
			=> GetDefinitionFor(typeof(T));

		public IMessageDefinition GetDefinitionFor(Type t)
			=> this.Cache.Retrieve(t, () => {
				string type;
				var msgProps = new List<IMessageProperty>();

				if (!t.HasAttribute<MessageAttribute>(out var msgAttrib)) throw new Exceptions.DecoratorException("unable 2 find");

				type = msgAttrib.Type;

				var repAttribs = t.GetAttributesOf<RepeatableAttribute>();

				foreach (var i in t.GetPositions()) {
					var pos = i.GetAttributesOf<PositionAttribute>()[0];
					var required = i.GetAttributesOf<RequiredAttribute>().Length > 0;
					if (i.GetAttributesOf<OptionalAttribute>().Length > 0) required = false;

					msgProps.Add(new MessageProperty(pos.Position, required, i.PropertyType, i));
				}

				var repeatable = repAttribs.Length > 0;

				return new MessageDefinition(type, msgProps, repeatable);
			});

		public T DeserializeToType<T>(BaseMessage m)
			where T : new()
			=> (T)DeserializeToType(typeof(T), m);

		public object DeserializeToType(Type t, BaseMessage m) {
			// we assume that QualifiesAsType has already been called

			var instance = Activator.CreateInstance(t);

			var def = this.GetDefinitionFor(t);

			foreach (var i in def.Properties) {
				i.Set(instance, m.Arguments[i.Position]);
			}

			return instance;
		}

		public bool QualifiesAsType<T>(BaseMessage m)
			=> QualifiesAsType(typeof(T), m);

		public bool QualifiesAsType(Type t, BaseMessage m) {
			if (!t.HasAttribute<MessageAttribute>(out var _)) return false;

			var def = this.GetDefinitionFor(t);

			if (m?.Type != def.Type) return false;

			if (m?.Count != def.MaxCount) return false;

			foreach (var i in def.Properties)
				if (i.PropertyType != m?.Arguments[i.Position]?.GetType())
					return false;

			return true;
		}

		public bool QualifiesAsRepeatableType<T>(BaseMessage m)
			=> QualifiesAsRepeatableType(typeof(T), m);

		public bool QualifiesAsRepeatableType(Type t, BaseMessage m) {
			if (!t.HasAttribute<MessageAttribute>(out var _)) return false;

			var def = this.GetDefinitionFor(t);
			
			if (def.Type != m.Type ||

				!def.Repeatable ||

			// prevent divide by zero exceptions lol
				def.MaxCount == 0 ||

				m.Count % def.MaxCount != 0) return false;
			
			for (uint i = 0; i < m.Count / def.MaxCount; i++) {
				var args = new object[def.MaxCount];

				Array.Copy(m.Arguments, i * def.MaxCount, args, 0, def.MaxCount);

				var msgCopy = new BasicMessage(m.Type, args);

				if (!QualifiesAsType(t, msgCopy)) return false;
			}

			return true;
		}

		public IEnumerable<T> DeserializeRepeatableToType<T>(BaseMessage m) where T : new()
			=> CastFrom<T>(this.DeserializeRepeatableToType(typeof(T), m));

		public IEnumerable<object> DeserializeRepeatableToType(Type t, BaseMessage m) {
			var def = this.GetDefinitionFor(t);

			for (uint i = 0; i < m.Count / def.MaxCount; i++) {
				var args = new object[def.MaxCount];

				Array.Copy(m.Arguments, i * def.MaxCount, args, 0, def.MaxCount);

				var msgCopy = new BasicMessage(m.Type, args);

				yield return DeserializeToType(t, msgCopy);
			}
		}

		private static IEnumerable<T> CastFrom<T>(IEnumerable<object> objs) {
			foreach (var i in objs)
				yield return (T)i;
		}
	}
}