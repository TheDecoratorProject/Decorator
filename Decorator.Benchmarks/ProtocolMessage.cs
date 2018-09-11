// credit: atillabyte ~ https://gist.github.com/atillabyte/f6f7062fdf1d9965cf019ac8e07a9241

namespace ProtocolMessage {
	[Message("say")]
	public class Chat {
		[Position(0), Required]
		public int PlayerId { get; set; }

		[Position(1), Required]
		public string Message { get; set; }

		[Position(2), Optional]
		public int Colour { get; set; } = 28;

		public Chat() {
		}
	}
}

namespace ProtocolMessage {
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Reflection.Emit;

	public class ProtocolMessageManager {
		public Dictionary<int, ProtocolMessage> ProtocolMessages { get; set; }
			= new Dictionary<int, ProtocolMessage>();

		public ProtocolMessageManager() {
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (var assembly in assemblies) {
				var types = assembly.GetTypes();

				foreach (var type in types) {
					if (type.IsDefined(typeof(Message), true)) {
						this.ProtocolMessages.Add(type.GetHashCode(), new ProtocolMessage(type));
					}
				}
			}
		}

		public T Convert<T>(object[] array) {
			if (array == null || array.Length == 0) {
				throw new ProtocolMessageException(string.Format("The message for type '{0}' could not be converted as the specified array is null or empty.",
					typeof(T).AssemblyQualifiedName));
			}

			var type = typeof(T);
			var instance = (T)InstanceCache.CreateInstance<object>(type);

			if (this.ProtocolMessages.TryGetValue(type.GetHashCode(), out var message)) {
				foreach (var property in message.Properties) {
					var position = property.Position;

					var required = property.Required;
					var optional = property.Optional;

					if (array.Length - 1 < position.Index || position.Index > array.Length - 1) {
						if (property.Required)
							throw new ProtocolMessageException(
								string.Format("The message could not be converted as the specified array does not contain index {0} for the required property '{1}'",
								position.Index, property.PropertyInfo.Name));

						if (property.Optional)
							continue;
					}

					property.PropertyInfo.SetValue(instance, array[position.Index]);
				}
			} else {
				this.ProtocolMessages.Add(type.GetHashCode(), new ProtocolMessage(type));

				return this.Convert<T>(array);
			}

			return instance;
		}
	}

	public class ProtocolMessage {
		public string MessageType { get; internal set; }

		public List<ProtocolProperty> Properties { get; internal set; }
			= new List<ProtocolProperty>();

		public ProtocolMessage(Type type) {
			this.MessageType = type.GetAttribute<Message>().MessageType;

			foreach (var property in PropertyCache.GetCachedProperties(type)) {
				var position = property.GetCustomAttribute<Position>();
				var required = property.GetCustomAttribute<Required>();
				var optional = property.GetCustomAttribute<Optional>();

				if (position == null)
					throw new ProtocolMessageException(string.Format("The required position attribute missing on property '{0}' of type '{1}'",
						property.Name, type.AssemblyQualifiedName));

				if (required != null && optional != null)
					throw new ProtocolMessageException(string.Format("The property '{0}' of type '{1}' cannot simultaneously be required and optional.",
						property.Name, type.AssemblyQualifiedName));

				if (required == null && optional == null)
					throw new ProtocolMessageException(string.Format("The property '{0}' of type '{1}' must contain either tbe required or optional attribute.",
						property.Name, type.AssemblyQualifiedName));

				this.Properties.Add(new ProtocolProperty() {
					PropertyInfo = property,
					Position = position,
					Optional = optional != null,
					Required = required != null
				});
			}
		}
	}

	public class ProtocolProperty {
		public PropertyInfo PropertyInfo { get; set; }
		public Position Position { get; set; }

		public bool Required { get; set; } = false;
		public bool Optional { get; set; } = false;
	}

	internal static class ExpressionHelpers {
		public static T GetAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute {
			var attributes = provider.GetCustomAttributes(typeof(T), true);
			return attributes.Length > 0 ? attributes[0] as T : null;
		}

		public static bool IsRequired<T, V>(this Expression<Func<T, V>> expression) {
			if (!(expression.Body is MemberExpression memberExpression))
				throw new InvalidOperationException("Expression must be a member expression");

			return memberExpression.Member.GetAttribute<Required>() != null;
		}

		public static bool IsOptional<T, V>(this Expression<Func<T, V>> expression) {
			if (!(expression.Body is MemberExpression memberExpression))
				throw new InvalidOperationException("Expression must be a member expression");

			return memberExpression.Member.GetAttribute<Optional>() != null;
		}
	}

	internal static class PropertyCache {
		internal static Dictionary<int, PropertyInfo[]> cache = new Dictionary<int, PropertyInfo[]>();

		public static PropertyInfo[] GetCachedProperties(this Type type) {
			if (cache.TryGetValue(type.GetHashCode(), out var props))
				return props;

			props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			cache.Add(type.GetHashCode(), props);

			return props;
		}
	}

	internal static class InstanceCache {
		public static T CreateInstance<T>(Type type) where T : class {
			if (!InstanceCacheStorage<T>.Cache.TryGetValue(type.FullName, out var function)) {
				var dynMethod = new DynamicMethod("$ProtoMessage_" + type.Name, type, null, type);
				var ilGen = dynMethod.GetILGenerator();

				ilGen.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
				ilGen.Emit(OpCodes.Ret);

				function = (Func<T>)dynMethod.CreateDelegate(typeof(Func<T>));
				InstanceCacheStorage<T>.Cache[type.FullName] = function;
			}

			return function();
		}
	}

	internal static class InstanceCacheStorage<T> {
		internal static ConcurrentDictionary<string, Func<T>> Cache = new ConcurrentDictionary<string, Func<T>>();
	}

	[Serializable]
	public class ProtocolMessageException : Exception {
		public ProtocolMessageException(string message) : base(message) {
		}
	}

	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	public sealed class Optional : Attribute {
		public Optional() {
		}
	}

	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	public sealed class Required : Attribute {
		public Required() {
		}
	}

	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	public sealed class Position : Attribute {
		public int Index { get; private set; }

		public Position(int index) {
			this.Index = index;
		}
	}

	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	public sealed class Message : Attribute {
		public Message(string messageType) {
			if (string.IsNullOrEmpty(messageType))
				throw new ProtocolMessageException("A valid non-empty message type must be specified for messages.");

			this.MessageType = messageType;
		}

		public string MessageType { get; private set; }
	}
}