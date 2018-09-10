using Decorator.Attributes;

using System;
using System.Collections.Generic;

namespace Decorator {

	public class MessageManager {

		public MessageManager()
			=> this.Cache = new Cache<Type, MessageDefinition>();

		public Cache<Type, MessageDefinition> Cache { get; }

		public MessageDefinition GetDefinitionFor<T>()
			=> GetDefinitionFor(typeof(T));

		public T DeserializeToType<T>(BaseMessage m)
					where T : new()
			=> (T)DeserializeToType(typeof(T), m);

		public bool QualifiesAsType<T>(BaseMessage m)
			=> QualifiesAsType(typeof(T), m);

		public bool QualifiesAsRepeatableType<T>(BaseMessage m)
					=> QualifiesAsRepeatableType(typeof(T), m);

		public IEnumerable<T> DeserializeRepeatableToType<T>(BaseMessage m) where T : new()
			=> CastFrom<T>(this.DeserializeRepeatableToType(typeof(T), m));

		public MessageDefinition GetDefinitionFor(Type t)
							=> this.Cache.Retrieve(t, () => {

								if (!t.HasAttribute<MessageAttribute>(out var msgAttrib)) throw new Exceptions.DecoratorException("unable 2 find");

								var type = msgAttrib.Type;

								var repAttribs = t.GetAttributesOf<RepeatableAttribute>();

								var msgProps = new List<MessageProperty>();

								foreach (var i in t.GetPositions())
									msgProps.Add(new MessageProperty(
										i.GetAttributesOf<PositionAttribute>()[0].Position, // pos
										i.GetAttributesOf<RequiredAttribute>().Length > 0 || // required
										!(i.GetAttributesOf<OptionalAttribute>().Length > 0),
										i.PropertyType,
										i));

								return new MessageDefinition(type, msgProps, repAttribs.Length > 0);
							});

		public bool QualifiesAsType(Type t, BaseMessage m) {
			if (!t.HasAttribute<MessageAttribute>(out var _)) return false;

			return QualifiesAsTypeWithoutMessageAttributeCheck(t, m);
		}

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

				if (!QualifiesAsTypeWithoutMessageAttributeCheck(t, msgCopy)) return false;
			}

			return true;
		}

		public object DeserializeToType(Type t, BaseMessage m) {
			// we assume that QualifiesAsType has already been called

			var instance = ReflectionHelper.Create(t)();

			var def = this.GetDefinitionFor(t);

			foreach (var i in def.Properties) {
				i.Set(instance, m.Arguments[i.Position]);
			}

			return instance;
		}

		public IEnumerable<object> DeserializeRepeatableToType(Type t, BaseMessage m) {
			var def = this.GetDefinitionFor(t);

			for (uint i = 0; i < m.Count / def.MaxCount; i++) {
				var args = new object[def.MaxCount];

				Array.Copy(m.Arguments, i * def.MaxCount, args, 0, def.MaxCount);

				var msgCopy = new BasicMessage(m.Type, args);

				yield return DeserializeToType(t, msgCopy);
			}
		}

		private bool QualifiesAsTypeWithoutMessageAttributeCheck(Type t, BaseMessage m) {
			var def = this.GetDefinitionFor(t);

			if (m?.Type != def.Type ||
				m?.Count != def.MaxCount) return false;

			foreach (var i in def.Properties)
				if (i.PropertyType != m?.Arguments[i.Position]?.GetType())
					return false;

			return true;
		}

		private static IEnumerable<T> CastFrom<T>(IEnumerable<object> objs) {
			foreach (var i in objs)
				yield return (T)i;
		}
	}
}