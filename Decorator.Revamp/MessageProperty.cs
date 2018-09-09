using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator {

	public interface IMessageDefinition {
		string Type { get; }

		uint MaxCount { get; }

		IMessageProperty[] Properties { get; }
	}

	public class MessageDefinition : IMessageDefinition {

		public MessageDefinition(string type, IEnumerable<IMessageProperty> props) {
			this.Type = type;
			this.Properties = props.ToArray();

			uint maxPos = 0;
			foreach (var i in this.Properties)
				if (i.Position > (maxPos > 0 ? maxPos - 1 : 0))
					maxPos = i.Position + 1;

			this.MaxCount = maxPos;
		}

		public string Type { get; }

		public uint MaxCount { get; }

		public IMessageProperty[] Properties { get; }
	}

	public interface IMessageProperty {
		bool Repeatable { get; }
		uint Position { get; }
		TypeRequiredness State { get; }
		Type PropertyType { get; }
		PropertyInfo PropertyInfo { get; }
	}

	public enum TypeRequiredness {
		Required, Optional
	}

	public class MessageProperty : IMessageProperty {

		public MessageProperty(bool rep, uint pos, bool req, Type propType, PropertyInfo propInf) {
			this.Repeatable = rep;
			this.Position = pos;
			this.State = req ? TypeRequiredness.Required : TypeRequiredness.Optional;
			this.PropertyType = propType;
			this.PropertyInfo = propInf;
		}

		public bool Repeatable { get; }

		public uint Position { get; }

		public TypeRequiredness State { get; }

		public Type PropertyType { get; }

		public PropertyInfo PropertyInfo { get; }
	}
}