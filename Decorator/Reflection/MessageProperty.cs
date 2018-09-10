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
		}

		private Func<object, object[], object> _propSetRaw;
		private Func<object, object[], object> _propGetRaw;

		private Func<object, object[], object> _propSet {
			get {
				if (this._propSetRaw == default)
					this._propSetRaw = IL.Wrap(this.PropertyInfo.GetSetMethod());

				return this._propSetRaw;
			}
		}

		private Func<object, object[], object> _propGet {
			get {
				if (this._propGetRaw == default)
					this._propGetRaw = IL.Wrap(this.PropertyInfo.GetGetMethod());

				return this._propGetRaw;
			}
		}

		public uint Position { get; }
		public TypeRequiredness State { get; }
		public Type PropertyType { get; }

		public PropertyInfo PropertyInfo { get; }

		public object Get(object instance)
			=> this._propGet(instance, new object[] { });

		public void Set(object instance, object value)
			=> this._propSet(instance, new[] { value });
	}
}