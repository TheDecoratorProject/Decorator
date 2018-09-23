using Decorator.Attributes;
using Decorator.Caching;

using System;
using System.Linq;
using System.Reflection;

namespace Decorator {

	internal static class MessageManager {

		static MessageManager() {
			Definitions = new HashcodeDictionary<Type, MessageDefinition>();

			foreach (var i in
				AppDomain.CurrentDomain.GetAssemblies()
					.Select(assembly => assembly.GetTypes())
					.SelectMany(type => type)
					.Where(type => type.IsDefined(typeof(MessageAttribute), true)))
				GetDefinitionForType(i);
		}

		private static HashcodeDictionary<Type, MessageDefinition> Definitions;

		internal static MessageDefinition GetDefinitionFor<T>()
			=> GetDefinitionForType(typeof(T));

		internal static MessageDefinition GetDefinitionForType(Type type) {
			// cache
			if (Definitions.TryGetValue(type, out var res)) return res;

			// if it is a message
			if (!AttributeCache<MessageAttribute>.TryHasAttribute(type, out var msgAttrib)) {
				res = default;
				Definitions.TryAdd(type, res);
				return res;
			}

			var repeatable = AttributeCache<RepeatableAttribute>.TryHasAttribute(type, out var _);

			// store properties
			var props = type.GetProperties();

			var max = 0;
			var msgProps = new MessageProperty[props.Length];

			for (var j = 0; j < props.Length; j++) {
				var i = props[j];

				if (HandleItem(i, out var prop))
					msgProps[max++] = prop;
			}

			// resize the array if needed
			if (msgProps.Length != max) {
				var newMsgProps = new MessageProperty[max];
				Array.Copy(msgProps, 0, newMsgProps, 0, max);
				msgProps = newMsgProps;
			}

			var msgDef = new MessageDefinition(
					msgAttrib[0].Type,
					msgProps,
					repeatable
				);

			Definitions.TryAdd(type, msgDef);
			return msgDef;
		}

		private static bool HandleItem(PropertyInfo i, out MessageProperty prop) {
			if (AttributeCache<PositionAttribute>.TryHasAttribute(i, out var posAttrib)) {
				var required = AttributeCache<RequiredAttribute>.TryHasAttribute(i, out var _);
				var optional = AttributeCache<OptionalAttribute>.TryHasAttribute(i, out var _);

				if (!required && !optional)
					required = true;
				else if (optional)
					required = false;

				prop = new MessageProperty(
						posAttrib[0].Position,
						required,
						i
					);
				return true;
			}

			prop = default;
			return false;
		}
	}
}