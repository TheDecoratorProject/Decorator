using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator
{
	internal class MessageDefinition
	{
		public MessageDefinition(string type, IEnumerable<MessageProperty> props, bool rep)
		{
			Repeatable = rep;
			Type = type;
			Properties = props.ToArray();

			uint maxPos = 0;
			foreach (var i in Properties)
				if (i.Position >= (maxPos > 0 ? maxPos - 1 : 0))
					maxPos = i.Position + 1;

			MaxCount = maxPos;
			IntMaxCount = (int)MaxCount;
		}

		public string Type;
		public MessageProperty[] Properties;
		public uint MaxCount;

		public bool Repeatable;

		internal int IntMaxCount;
	}

	internal enum TypeRequiredness
	{
		Required,

		Optional
	}

	internal class MessageProperty
	{
		public MessageProperty(uint pos, bool req, PropertyInfo propInf)
		{
			Position = pos;
			State = req ? TypeRequiredness.Required : TypeRequiredness.Optional;
			PropertyInfo = propInf;

			_propSetRaw = PropertyInfo.GetSetMethodByExpression();
			_propGetRaw = PropertyInfo.GetGetMethod().ILWrap();

			Type = propInf.PropertyType;
			TypeHashcode = Type.GetHashCode();

			IntPos = (int)Position;
		}

		public uint Position;

		public TypeRequiredness State;

		public PropertyInfo PropertyInfo;
		public Type Type;
		public int TypeHashcode;

		internal int IntPos;

		private readonly Action<object, object> _propSetRaw;
		private readonly ILFunc _propGetRaw;

		public object Get(object instance)
			=> _propGetRaw(instance, default);

		public void Set(object instance, object value)
			=> _propSetRaw(instance, value);
	}
}