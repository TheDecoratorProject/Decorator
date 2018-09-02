using Decorator.Attributes;

using System;

namespace Decorator {
	// TODO: use enums instead of out strings

	public static partial class Deserializer {

		/// <summary>
		/// Attempts to deserialize a message to a class.
		/// </summary>
		/// <typeparam name="T">The class to be deserializing the Message to</typeparam>
		/// <param name="msg">The Message needing deserialization</param>
		/// <param name="res">The result, T</param>
		/// <param name="failErrMsg">A reason for failure.</param>
		/// <returns>If it succeeded.</returns>
		public static bool TryDeserialize<T>(Message msg, out T res, out string failErrMsg)
			where T : class, new() {
			// ensure some basic things

			res = default;
			failErrMsg = default;

			ReflectionHelper.CheckNull(msg, nameof(msg));

			var msgAttrib = ReflectionHelper.EnsureAttributeGet<MessageAttribute, T>();
			if (msgAttrib.Type != msg.Type) return OneLinerFail("The base message types aren't equal.", out failErrMsg);

			// if there's a limit on the amount of arguments set for the item

			if (ReflectionHelper.TryGetAttributeOf<ArgumentLimitAttribute>(typeof(T), out var limit) &&
				msg?.Args?.Length > limit.ArgLimit)
				return OneLinerFail($"Surpassed the maximum amount of args bound by the {nameof(ArgumentLimitAttribute)}", out failErrMsg);

			// loop through every property

			res = Activator.CreateInstance<T>();

			foreach (var i in ReflectionHelper.GetPropertiesWithAttribute<PositionAttribute>(typeof(T))) {
				// if it has a position
				if (ReflectionHelper.TryGetAttributeOf<PositionAttribute>(i, out var posAttrib))

					// if there's an argument in the message for it
					if (posAttrib.Position >= 0 && msg.Args?.Length > posAttrib.Position &&
						i.PropertyType.IsAssignableFrom(ReflectionHelper.GetTypeOf(msg?.Args?[posAttrib.Position]))) {
						i.SetValue(res, msg.Args[posAttrib.Position]);
					} else {
						// if there's no optional attribute, or there's a required attribute
						if (!ReflectionHelper.TryGetAttributeOf<OptionalAttribute>(i, out var _) ||
							ReflectionHelper.TryGetAttributeOf<RequiredAttribute>(i, out var __)) {
							// yell at 'em
							return OneLinerFail($"Unable to set {i.Name}", out failErrMsg);
						}
					}
			}

			return true;
		}
	}
}