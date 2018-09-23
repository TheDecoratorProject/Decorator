using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator {

	internal class MessageDefinition {

		public MessageDefinition(string type, IEnumerable<MessageProperty> props, bool rep) {
			this.Repeatable = rep;
			this.Type = type;
			this.Properties = props.ToArray();

			uint maxPos = 0;
			foreach (var i in this.Properties)
				if (i.Position >= (maxPos > 0 ? maxPos - 1 : 0))
					maxPos = i.Position + 1;

			this.MaxCount = maxPos;
			this.IntMaxCount = (int)this.MaxCount;
		}

		public string Type;
		public MessageProperty[] Properties;
		public uint MaxCount;

		public bool Repeatable;

		internal int IntMaxCount;
	}

	internal enum TypeRequiredness {
		Required,

		Optional
	}

	internal class MessageProperty {

		public MessageProperty(uint pos, bool req, PropertyInfo propInf) {
			this.Position = pos;
			this.State = req ? TypeRequiredness.Required : TypeRequiredness.Optional;
			this.PropertyInfo = propInf;

			this._propSetRaw = this.PropertyInfo.GetSetMethodByExpression();
			this._propGetRaw = this.PropertyInfo.GetGetMethod().ILWrap();

			this.IntPos = (int)this.Position;
		}

		public uint Position;

		public TypeRequiredness State;

		public PropertyInfo PropertyInfo;

		internal int IntPos;

		private readonly Action<object, object> _propSetRaw;
		private readonly ILFunc _propGetRaw;

		public object Get(object instance)
			=> this._propGetRaw(instance, default);

		public void Set(object instance, object value)
			=> this._propSetRaw(instance, value);
	}
}