using Decorator.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator {

	public static class Deserializer {

		/// <summary>Deserialize a Message, and find the appropiate DeserializedHandlerAttribute that takes the type of the deserialized message.</summary>
		/// <typeparam name="T">The class to look for methods in</typeparam>
		/// <param name="eventClass">An instance of the class. Leave it to null to find static methods</param>
		/// <param name="msg">The message to be deserializing</param>
		/// <param name="extraParams">Additional parameters that you wish to pass into the function</param>
		/// <returns>The amount of events invoked</returns>
		/// <exception cref="CustomAttributeFormatException">DeserializedHandlerAttribute</exception>
		public static int DeserializeToEvent<T>(T eventClass, Message msg, params object[] extraParams)
			where T : class, new()
			=> InternalDeserializeToEvent(eventClass, msg, extraParams).GetResult();

		/// <summary>Attempts to deserialize the Message to an IEnumerable of type T</summary>
		/// <typeparam name="T">The type of message that it needs to deserialize</typeparam>
		/// <param name="msg">The message to deserialize</param>
		/// <returns>If it succeeded</returns>
		public static IEnumerable<T> DeserializeToIEnumerable<T>(Message msg)
			where T : class, new()
			=> InternalDeserializeToIEnumerable<T>(msg).GetResult();

		/// <summary>
		/// Attempts to deserialize a message to a class.
		/// </summary>
		/// <typeparam name="T">The class to be deserializing the Message to</typeparam>
		/// <param name="msg">The Message needing deserialization</param>
		/// <returns>If it succeeded.</returns>
		public static T Deserialize<T>(Message msg)
			where T : class, new()
			=> InternalDeserialize<T>(msg).GetResult();

		internal static FastException<int> InternalDeserializeToEvent<T>(T eventClass, Message msg, params object[] extraParams)
							where T : class, new() {
			if (msg == default(T))
				return new FastException<int>(new ArgumentNullException(nameof(msg)));

			var exceptions = new List<Exception>();

			var methodInvokes = 0;

			// loop through every deserializable handler
			foreach (var method in ReflectionHelper.GetMethodsWithAttribute<DeserializedHandlerAttribute>(eventClass?.GetType() ?? typeof(T))) {
				var res = DeserializeToMethod(eventClass, msg, method, extraParams);

				if (res.ThrownException) {
					exceptions.Add(res.Exception);
				} else methodInvokes++;
			}

			if (methodInvokes == 0)
				return new FastException<int>(new AggregateException(exceptions));

			return new FastException<int>(methodInvokes);
		}

		internal static FastException<IEnumerable<T>> InternalDeserializeToIEnumerable<T>(Message msg)
			where T : class, new() {
			if (msg == default(T))
				return new FastException<IEnumerable<T>>(new ArgumentNullException(nameof(msg)));

			// make sure it supports IEnumerable deserialization
			//TODO: helper functions
			if (ReflectionHelper.GetAttributeOf<RepeatableAttribute>(typeof(T)) == default)
				return new FastException<IEnumerable<T>>(new CustomAttributeFormatException($"Could not find a {nameof(RepeatableAttribute)} on {typeof(T).FullName}"));

			if (ReflectionHelper.GetAttributeOf<MessageAttribute>(typeof(T)) == default)
				return new FastException<IEnumerable<T>>(new CustomAttributeFormatException($"Could not find a {nameof(MessageAttribute)} on {typeof(T).FullName}"));

			var msgPosLength = ReflectionHelper.GetLargestPositionAttribute(typeof(T))
							 + 1;

			// ensure that the message length is just a bunch of the same message
			if (msg.Args.Length % msgPosLength != 0) return new FastException<IEnumerable<T>>(new BaseMessageInequalityException("Uneven amount of message params"));

			var innerMessages = msg.Args.Length / msgPosLength;

			var resultingMessages = new List<T>();

			// for every "inner message"
			for (var i = 0; i < innerMessages; i++) {
				// copy it and try to deserialize it
				var args = new object[msgPosLength];
				Array.Copy(msg.Args, i * msgPosLength, args, 0, msgPosLength);

				resultingMessages.Add(Deserialize<T>(new Message(msg.Type, args)));
			}

			return new FastException<IEnumerable<T>>(resultingMessages);
		}

		internal static FastException<T> InternalDeserialize<T>(Message msg)
							where T : class, new() {
			if (msg == default(T))
				return new FastException<T>(new ArgumentNullException(nameof(msg)));

			var msgAttrib = ReflectionHelper.GetAttributeOf<MessageAttribute>(typeof(T));
			if (msgAttrib == default)
				return new FastException<T>(new CustomAttributeFormatException($"Could not find a {nameof(MessageAttribute)} on {typeof(T).FullName}"));

			if (msgAttrib.Type != msg.Type) return new FastException<T>(new BaseMessageInequalityException("The base message types aren't equal."));

			// if there's a limit on the amount of arguments set for the item

			if (ReflectionHelper.TryGetAttributeOf<ArgumentLimitAttribute>(typeof(T), out var limit) &&
				msg?.Args?.Length > limit.ArgLimit)
				return new FastException<T>(new ArgumentException($"Surpassed the maximum amount of args bound by the {nameof(ArgumentLimitAttribute)}"));

			// loop through every property

			var res = Activator.CreateInstance<T>();

			foreach (var property in ReflectionHelper.GetPropertiesWithAttribute<PositionAttribute>(typeof(T))) {
				// if it has a position
				if (ReflectionHelper.TryGetAttributeOf<PositionAttribute>(property, out var posAttrib))
					if (
					posAttrib.Position >= 0 &&
					msg.Args?.Length > posAttrib.Position) {
						// if there's an argument in the message for it
						if (property.PropertyType.IsAssignableFrom(msg?.Args?[posAttrib.Position]?.GetType() ?? typeof(T))) {
							property.SetValue(res, msg.Args[posAttrib.Position]);
						} else {
							// if there's no optional attribute, or there's a required attribute
							if (!ReflectionHelper.TryGetAttributeOf<OptionalAttribute>(property, out var _) ||
								ReflectionHelper.TryGetAttributeOf<RequiredAttribute>(property, out var __)) {
								// yell at 'em
								return new FastException<T>(new InabilityToSetValueException($"Unable to set the value of ({property.Name}) to the value ({msg?.Args[posAttrib.Position]}) - consider putting an [{nameof(OptionalAttribute)}] on the property!"));
							}
						}
					} else return new FastException<T>(new InvalidMessageCountException($"The message doesn't match the position attributes"));
			}

			return new FastException<T>(res);
		}

		private static bool CanInvokeIEnumerableMethod(Type desType, Message msg) {
			//TODO: check if it is an IEnumerable<> itself and not if it inherits IEnumerable

			if (desType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable))) {
				var genArg = desType.GenericTypeArguments[0];

				if (ReflectionHelper.TryGetAttributeOf<MessageAttribute>(genArg, out var msgAttrib))
					return msgAttrib?.Type == msg?.Type;
			}

			return false;
		}

		private static FastException<bool> DeserializeToMethod<T>(T eventClass, Message msg, MethodInfo method, params object[] extraParams)
							where T : class, new() {
			// get the args
			var parameters = method.GetParameters();

			if (parameters == null)
				return new FastException<bool>(new ArgumentNullException(nameof(parameters), $"Parameters was null"));

			if (parameters?.Length < 1)
				return new FastException<bool>(new CustomAttributeFormatException($"Invalid [{nameof(DeserializedHandlerAttribute)}] - must have at least one parameter"));

			// get the type
			var deserializingType = parameters[0].ParameterType;

			// try to convert to an IEnumerable
			if (CanInvokeIEnumerableMethod(deserializingType, msg)) {
				var result = (IFastException)InvokeGenericDeserialize(nameof(InternalDeserializeToIEnumerable), msg, deserializingType.GenericTypeArguments[0]);

				if (result.ThrownException)
					return new FastException<bool>(result.Exception);

				InvokeMethod(eventClass, method, result.Result, extraParams);
				return new FastException<bool>(true);
			} else

				// if we didn't convert it to an ienumerable and we can des. to a desType
				if (!deserializingType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable))) {
				var result = (IFastException)InvokeGenericDeserialize(nameof(InternalDeserialize), msg, deserializingType);

				if (result.ThrownException)
					return new FastException<bool>(result.Exception);

				InvokeMethod(eventClass, method, result.Result, extraParams);
				return new FastException<bool>(true);
			}

			return new FastException<bool>(false);
		}

		private static object InvokeGenericDeserialize(string methodName, Message msg, Type deserializingType) {
			var parameters = new object[] { msg };

			var result = typeof(Deserializer)
							.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
							.MakeGenericMethod(deserializingType)
							.Invoke(null, parameters);

			return result;
		}

		private static void InvokeMethod<T>(T eventClass, MethodInfo meth, object invokItm, object[] extraParams)
					where T : class {
			var invokeArgs = new object[extraParams.Length + 1];

			Array.Copy(extraParams, 0, invokeArgs, 1, extraParams.Length);

			invokeArgs[0] = invokItm;

			meth.Invoke(eventClass, invokeArgs);
		}
	}
}