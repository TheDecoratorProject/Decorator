using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator {

	public class MessageDefinition {

		public MessageDefinition(string type, IEnumerable<MessageProperty> props, bool rep) {
			this.Repeatable = rep;
			this.Type = type;
			this.Properties = props.ToArray();

			uint maxPos = 0;
			foreach (var i in this.Properties)
				if (i.Position >= (maxPos > 0 ? maxPos - 1 : 0))
					maxPos = i.Position + 1;

			this.MaxCount = maxPos;
		}

		public string Type { get; }
		public MessageProperty[] Properties { get; }
		public uint MaxCount { get; }
		public bool Repeatable { get; }
	}

	public enum TypeRequiredness {
		Required, Optional
	}

	public class MessageProperty {

		public MessageProperty(uint pos, bool req, Type propType, PropertyInfo propInf) {
			this.Position = pos;
			this.State = req ? TypeRequiredness.Required : TypeRequiredness.Optional;
			this.PropertyType = propType;
			this.PropertyInfo = propInf;

			this._propSet = IL.Wrap(this.PropertyInfo.GetSetMethod());
		}

		private readonly Func<object, object[], object> _propSet;
		public uint Position { get; }
		public TypeRequiredness State { get; }
		public Type PropertyType { get; }

		public PropertyInfo PropertyInfo { get; }

		public void Set(object instance, object value)
			=> this._propSet(instance, new[] { value });
	}
}