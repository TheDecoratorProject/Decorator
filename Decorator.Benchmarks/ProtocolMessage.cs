namespace ProtocolMessage {
	using System;
	using System.Collections.Generic;

	using System.Linq;
	using System.Linq.Expressions;

	using System.Reflection;

	[Message("say")]
	public class Chat {

		[Position(0), Required]
		public int PlayerId { get; set; }

		[Position(1), Required]
		public string Message { get; set; }
	}

	public class ProtocolMessageManager {
		internal Dictionary<int, ProtocolMessage> ProtocolMessages { get; }
			= new Dictionary<int, ProtocolMessage>();

		public ProtocolMessageManager() {
			this.ProtocolMessages =
				AppDomain.CurrentDomain.GetAssemblies()
				.Select(assembly => assembly.GetTypes())
				.SelectMany(type => type)
				.Where(type => type.IsDefined(typeof(Message), true))
				.ToDictionary(kvp => kvp.GetHashCode(), kvp => new ProtocolMessage(kvp));
		}

		public T Convert<T>(object[] array) {
			if (array == null || array.Length == 0) {
				throw new ProtocolMessageException(string.Format("The message for type '{0}' could not be converted as the specified array is null or empty.",
					typeof(T).AssemblyQualifiedName));
			}

			var type = typeof(T);

			if (this.ProtocolMessages.TryGetValue(type.GetHashCode(), out var message)) {
				var instance = (T)InstanceCache.CreateInstance<object>(type);

				foreach (var property in message.Members) {
					var position = property.Position;

					var required = property.Required;
					var optional = property.Optional;

					if (array.Length - 1 < position.Index || position.Index > array.Length - 1) {
						if (property.Required)
							throw new ProtocolMessageException(
								string.Format("The message could not be converted as the specified array does not contain index {0} for the required property '{1}'",
								position.Index, property.MemberInfo.Name));
						if (property.Optional)
							continue;
					}
					
					property.SetValue(instance, array[position.Index]);
				}

				return instance;
			} else {
				this.ProtocolMessages.Add(type.GetHashCode(), new ProtocolMessage(type));
				return this.Convert<T>(array);
			}
		}
	}

	internal class ProtocolMessage {
		internal string MessageType { get; }

		internal List<IProtocolMember> Members
			= new List<IProtocolMember>();

		internal ProtocolMessage(Type type) {
			this.MessageType = type.GetAttribute<Message>().MessageType;

			foreach (var member in new List<MemberInfo>(PropertyCache.Get(type)).Union(FieldCache.Get(type))) {
				var position = member.GetCustomAttribute<Position>();
				var required = member.GetCustomAttribute<Required>();
				var optional = member.GetCustomAttribute<Optional>();

				if (position == null)
					throw new ProtocolMessageException(string.Format("The required position attribute missing on property or field '{0}' of type '{1}'",
						member.Name, type.FullName));

				if (required != null && optional != null)
					throw new ProtocolMessageException(string.Format("The property or field '{0}' of type '{1}' cannot simultaneously be required and optional.",
						member.Name, type.FullName));

				if (required == null && optional == null)
					throw new ProtocolMessageException(string.Format("The property or field '{0}' of type '{1}' must contain either tbe required or optional attribute.",
						member.Name, type.FullName));

				IProtocolMember entry = null;

				switch (member.MemberType) {
					case MemberTypes.Property:
					entry = new ProtocolProperty(member, type) {
						Position = position,
						Optional = optional != null,
						Required = required != null
					};
					break;
					case MemberTypes.Field:
					entry = new ProtocolField(member, type) {
						Position = position,
						Optional = optional != null,
						Required = required != null
					};
					break;
				}

				this.Members.Add(entry);
			}
		}
	}

	internal interface IProtocolMember {
		MemberInfo MemberInfo { get; set; }
		Position Position { get; set; }

		bool Required { get; set; }
		bool Optional { get; set; }

		void SetValue(object instance, object value);

		Action<object, object> SetMemberValue { get; }
	}

	internal class ProtocolProperty : IProtocolMember {
		public MemberInfo MemberInfo { get; set; }
		public Position Position { get; set; }

		public bool Required { get; set; }
		public bool Optional { get; set; }

		public Action<object, object> SetMemberValue { get; }

		public ProtocolProperty(MemberInfo property, Type type) {
			this.MemberInfo = property;
			this.SetMemberValue = ((PropertyInfo)property).GetSetMethodByExpression();
		}

		public void SetValue(object instance, object value) =>
			this.SetMemberValue(instance, value);
	}

	internal class ProtocolField : IProtocolMember {
		public MemberInfo MemberInfo { get; set; }
		public Position Position { get; set; }

		public bool Required { get; set; }
		public bool Optional { get; set; }

		public Action<object, object> SetMemberValue { get; }

		public ProtocolField(MemberInfo property, Type type) {
			this.MemberInfo = property;
			this.SetMemberValue = ((FieldInfo)property).GetSetMethodByExpression();
		}

		public void SetValue(object instance, object value) =>
			this.SetMemberValue(instance, value);
	}

	internal static class ExpressionHelpers {
		internal static T GetAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute =>
			(provider.GetCustomAttributes(typeof(T), true)?[0] as T) ?? null;
	}

	internal static class PropertyInfoExtensions {
		internal static Action<object, object> GetSetMethodByExpression(this PropertyInfo propertyInfo) {
			var _obj = typeof(object);

			var setMethodInfo = propertyInfo.GetSetMethod(true);
			var instance = Expression.Parameter(_obj, "instance");
			var value = Expression.Parameter(_obj, "value");
			var instanceCast = (!(propertyInfo.DeclaringType).GetTypeInfo().IsValueType) ? Expression.TypeAs(instance, propertyInfo.DeclaringType) : Expression.Convert(instance, propertyInfo.DeclaringType);
			var valueCast = (!(propertyInfo.PropertyType).GetTypeInfo().IsValueType) ? Expression.TypeAs(value, propertyInfo.PropertyType) : Expression.Convert(value, propertyInfo.PropertyType);

			return Expression.Lambda<Action<object, object>>(Expression.Call(instanceCast, setMethodInfo, valueCast), new ParameterExpression[] { instance, value }).Compile();
		}

		internal static Action<object, object> GetSetMethodByExpression(this FieldInfo fieldInfo) {
			var _obj = typeof(object);

			var instance = Expression.Parameter(_obj, "instance");
			var value = Expression.Parameter(_obj, "value");

			return Expression.Lambda<Action<object, object>>(Expression.Assign(Expression.Field(
				Expression.Convert(instance, fieldInfo.DeclaringType), fieldInfo),
				Expression.Convert(value, fieldInfo.FieldType)), instance, value).Compile();
		}
	}

	internal static class PropertyCache {
		internal static Dictionary<int, PropertyInfo[]> Cache = new Dictionary<int, PropertyInfo[]>();

		internal static PropertyInfo[] Get(Type type) {
			if (Cache.TryGetValue(type.GetHashCode(), out var properties))
				return properties;

			properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			Cache[type.GetHashCode()] = properties;

			return properties;
		}
	}

	internal static class FieldCache {
		internal static Dictionary<int, FieldInfo[]> Cache = new Dictionary<int, FieldInfo[]>();

		internal static FieldInfo[] Get(Type type) {
			if (Cache.TryGetValue(type.GetHashCode(), out var fields))
				return fields;

			fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			Cache[type.GetHashCode()] = fields;

			return fields;
		}
	}

	internal static class InstanceCache {
		internal static T CreateInstance<T>(Type type) where T : class {
			if (!InstanceCacheStorage<T>.Cache.TryGetValue(type.GetHashCode(), out var function)) {
				function = Expression.Lambda<Func<T>>(Expression.New(type)).Compile();
				InstanceCacheStorage<T>.Cache[type.GetHashCode()] = function;
			}

			return function();
		}
	}

	internal static class InstanceCacheStorage<T> where T : class {
		internal static Dictionary<int, Func<T>> Cache = new Dictionary<int, Func<T>>();
	}

	[Serializable]
	public sealed class ProtocolMessageException : Exception {
		public ProtocolMessageException(string message) : base(message) { }
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class Optional : Attribute { }

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class Required : Attribute { }

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class Position : Attribute {
		public int Index { get; private set; }

		public Position(int index) =>
			this.Index = index;
	}

	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	public sealed class Message : Attribute {
		public string MessageType { get; private set; }

		public Message(string messageType) {
			if (string.IsNullOrEmpty(messageType))
				throw new ProtocolMessageException("A valid non-empty message type must be specified for messages.");

			this.MessageType = messageType;
		}
	}
}