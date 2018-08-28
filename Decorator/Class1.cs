using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator
{
	// FOR REFERENCE ONLY
	// copying code from this is an awful idea

	/*
	public sealed class MessageAttribute : Attribute {
		public MessageAttribute(string name) {
			this.Type = name;
		}

		public string Type { get; set; }
	}

	public sealed class MessagePlaceAttribute : Attribute {
		public MessagePlaceAttribute(int location) {
			this.Location = location;
		}

		public int Location { get; set; }
	}

	public sealed class LimitAttribute : Attribute {
		public int SetLength {
			set {
				this.MinLength = value;
				this.MaxLength = value;
			}
		}

		public int MinLength { get; set; } = int.MinValue;
		public int MaxLength { get; set; } = int.MaxValue;
	}

	public interface IMessageConsulter {
		bool IsNull();
		string GetMessageType();
		int GetItemCount();
		object GetItemAt(int index);
		Type GetRawMessageType();
	}

	public interface IMessageWriter {
		void WriteObjectCount(int amt);

		void WriteObject(object obj, int index);
		void WriteType(string type);
	}
	
	public static class Parser {
		public static bool WriteMessage(object message, IMessageWriter writeTo) {
			if (writeTo == null) throw new ArgumentNullException(nameof(writeTo));

			var attrib = GetAttributeOf<MessageAttribute>(message.GetType());

			if (attrib != default(MessageAttribute) &&
				attrib != null) {
				var type = attrib.Type;

				writeTo.WriteType(type);
				writeTo.WriteObjectCount(GetMaxMessagePlace(message));

				foreach(var i in message.GetType().GetProperties()) {
					var mpa = GetAttributeOf<MessagePlaceAttribute>(i.GetType());

					if (mpa != default(MessagePlaceAttribute) &&
						mpa != null) {
						
					}
				}
			}

			return false;
		}

		private static int GetMaxMessagePlace(object obj) {
			var maxPlace = int.MinValue;

			foreach(var i in obj.GetType().GetProperties()) {
				var mpa = GetAttributeOf<MessagePlaceAttribute>(i.GetType());

				if (mpa != default(MessagePlaceAttribute) &&
					mpa != null) {

					if(mpa.Location > maxPlace) {
						maxPlace = mpa.Location;
					}
				}
			}

			return maxPlace + 1;
		}

		public static bool TryDeserializeGeneric(IMessageConsulter msg, out object result) {
			result = default(object);

			//TODO: loop through assemblies

			foreach (var k in GetDependentAssemblies(typeof(Parser).Assembly))
				foreach (var i in k.GetTypes())
					if (GetAttributeOf<MessageAttribute>(i) != null) {

						result = Activator.CreateInstance(i);

						var args = new object[] { msg, null };

						//TODO: consult stackoverflow
						var generic = typeof(Parser).GetMethod("TryDeserialize").MakeGenericMethod(i);
						var gmo = (bool)generic.Invoke(null, args);

						if (gmo) {
							result = args[1];
							return true;
						}
					}

			return false;
		}

		//NOTE: This method name is used as a string in places of this library.
		//		Be careful when changing the name.
		public static bool TryDeserialize<T>(IMessageConsulter msg, out T result) {
			result = default(T);

			if (msg == null) throw new ArgumentNullException(nameof(msg));
			
			var msgAttrib = GetAttributeOf<MessageAttribute>(typeof(T));

			if (msgAttrib != null &&
				msgAttrib.Type == msg.GetMessageType() &&
				msgAttrib != default(MessageAttribute)) {

				var msgPlaces = 0;

				result = (T)Activator.CreateInstance(typeof(T));

				foreach (var i in typeof(T).GetProperties()) {
					var mpa = GetAttributeOf<MessagePlaceAttribute>(i);
					
					if (mpa != default(MessagePlaceAttribute) &&
						mpa != null) {

						msgPlaces++;

						if (msg.GetItemCount() > mpa.Location) {
							i.SetValue(result, msg.GetItemAt(mpa.Location));
						} else return false;
					}
				}
				
				return msgPlaces == msg.GetItemCount();
			}

			return false;
		}

		private static T GetAttributeOf<T>(Type type) {
			if (type.GetCustomAttribute(typeof(T)) is T attribute) {
				if (attribute != null)
					return attribute;
			}

			return default(T);
		}

		private static T GetAttributeOf<T>(PropertyInfo type) {
			if (type.GetCustomAttribute(typeof(T)) is T attribute) {
				if (attribute != null)
					return attribute;
			}

			return default(T);
		}

		private static IEnumerable<Assembly> GetDependentAssemblies(Assembly analyzedAssembly) {
			return AppDomain.CurrentDomain.GetAssemblies()
				.Where(a => GetNamesOfAssembliesReferencedBy(a)
									.Contains(analyzedAssembly.FullName));
		}

		private static IEnumerable<string> GetNamesOfAssembliesReferencedBy(Assembly assembly) {
			return assembly.GetReferencedAssemblies()
				.Select(assemblyName => assemblyName.FullName);
		}
	}*/
	
}
