using System;
using System.Diagnostics;

namespace ProtocolMessage {

	[Message("say")]
	public class Chat {
		[Position(0), Required]
		public int PlayerId { get; set; }

		[Position(1), Required]
		public string Message { get; set; }

		public Chat() {

		}
	}

}

namespace ProtocolMessage {
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Reflection.Emit;

	public class ProtocolMessageManager {
		internal Dictionary<int, ProtocolMessage> ProtocolMessages { get; }
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

					property.SetValue(instance, array[position.Index]);
				}
			} else {
				this.ProtocolMessages.Add(type.GetHashCode(), new ProtocolMessage(type));
				return this.Convert<T>(array);
			}

			return instance;
		}
	}

	internal class ProtocolMessage {
		internal string MessageType { get; }

		internal List<ProtocolProperty> Properties { get; }
			= new List<ProtocolProperty>();

		internal ProtocolMessage(Type type) {
			this.MessageType = type.GetAttribute<Message>().MessageType;

			foreach (var property in PropertyCache.GetCachedProperties(type)) {
				var position = property.GetCustomAttribute<Position>();
				var required = property.GetCustomAttribute<Required>();
				var optional = property.GetCustomAttribute<Optional>();

				if (position == null)
					throw new ProtocolMessageException(string.Format("The required position attribute missing on property '{0}' of type '{1}'",
						property.Name, type.FullName));

				if (required != null && optional != null)
					throw new ProtocolMessageException(string.Format("The property '{0}' of type '{1}' cannot simultaneously be required and optional.",
						property.Name, type.FullName));

				if (required == null && optional == null)
					throw new ProtocolMessageException(string.Format("The property '{0}' of type '{1}' must contain either tbe required or optional attribute.",
						property.Name, type.FullName));

				this.Properties.Add(new ProtocolProperty(property, type) {
					Position = position,
					Optional = optional != null,
					Required = required != null
				});
			}
		}
	}

	internal class ProtocolProperty {
		internal PropertyInfo PropertyInfo { get; set; }
		internal Position Position { get; set; }

		internal bool Required { get; set; } = false;
		internal bool Optional { get; set; } = false;

		internal Func<object, object[], object> GetSetMethod { get; set; }

		internal void SetValue(object instance, object value) {
			this.GetSetMethod(instance, new[] { value });
		}

		internal ProtocolProperty(PropertyInfo property, Type type) {
			this.PropertyInfo = property;
			this.GetSetMethod = ILHelpers.Wrap(this.PropertyInfo.GetSetMethod());
		}
	}

	internal static class ExpressionHelpers {
		internal static T GetAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute {
			var attributes = provider.GetCustomAttributes(typeof(T), true);
			return attributes.Length > 0 ? attributes[0] as T : null;
		}
	}

	internal static class ILHelpers {
		internal static Func<object, object[], object> Wrap(MethodInfo method) {
			var dm = new DynamicMethod(method.Name, typeof(object), new[] { typeof(object), typeof(object[]) }, method.DeclaringType, true);
			var il = dm.GetILGenerator();

			if (!method.IsStatic) {
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Unbox_Any, method.DeclaringType);
			}

			var parameters = method.GetParameters();
			for (var i = 0; i < parameters.Length; i++) {
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldc_I4, i);
				il.Emit(OpCodes.Ldelem_Ref);
				il.Emit(OpCodes.Unbox_Any, parameters[i].ParameterType);
			}

			il.EmitCall(method.IsStatic || method.DeclaringType.IsValueType ?
				OpCodes.Call : OpCodes.Callvirt, method, null);

			if (method.ReturnType == null || method.ReturnType == typeof(void))
				il.Emit(OpCodes.Ldnull);
			else if (method.ReturnType.IsValueType)
				il.Emit(OpCodes.Box, method.ReturnType);

			il.Emit(OpCodes.Ret);

			return (Func<object, object[], object>)dm.CreateDelegate(typeof(Func<object, object[], object>));
		}
	}

	internal static class PropertyCache {
		internal static ConcurrentDictionary<int, PropertyInfo[]> Cache = new ConcurrentDictionary<int, PropertyInfo[]>();

		internal static PropertyInfo[] GetCachedProperties(this Type type) {
			if (Cache.TryGetValue(type.GetHashCode(), out var props))
				return props;

			props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			Cache[type.GetHashCode()] = props;

			return props;
		}
	}

	internal static class InstanceCache {
		public static T CreateInstance<T>(Type type) where T : class {
			if (!InstanceCacheStorage<T>.Cache.TryGetValue(type.FullName, out var function)) {
				var dynMethod = new DynamicMethod(type.Name, type, null, type);
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
		public static ConcurrentDictionary<string, Func<T>> Cache = new ConcurrentDictionary<string, Func<T>>();
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