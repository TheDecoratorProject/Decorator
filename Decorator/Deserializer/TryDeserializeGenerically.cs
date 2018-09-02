using System;
using System.Reflection;

namespace Decorator {
	// TODO: use enums instead of out strings

	public static partial class Deserializer {

		/// <summary>
		/// Invokes the TryDeserilize method, accepting Types instead of a compile-time T constraint.
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="desType">The type to deserialize to</param>
		/// <param name="res">The result</param>
		/// <param name="failErrMsg">The reason for failure</param>
		/// <returns>If it succeded.</returns>
		public static bool TryDeserializeGenerically(Message msg, Type desType, out object res, out string failErrMsg) {
			var args = new object[] { msg, null, null };

			var generic = typeof(Deserializer).GetMethod(nameof(TryDeserialize), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(desType);
			var gmo = (bool)generic.Invoke(null, args);

			res = args[1];
			failErrMsg = (string)args[2];

			return gmo;
		}
	}
}