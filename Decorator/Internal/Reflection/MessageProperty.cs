using Decorator.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator
{
	internal class MessageDefinition
	{
		public MessageDefinition(string type, IEnumerable<MessageProperty> props)
		{
			Type = type;
			Properties = props
				.OrderBy(x => x.Position)
				.ToArray();

			if(Properties.Length > 0)
				MaximumSize = Properties[Properties.Length - 1].PositionInt;
		}

		public string Type;
		public MessageProperty[] Properties;
		public int MaximumSize;
	}

	internal enum TypeTreatment
	{
		Required,

		Optional
	}

	internal class MessageProperty
	{
		public MessageProperty(uint pos, PropertyInfo property)
		{
			Position = pos;
			PositionInt = (int)pos;

			// if it has [Optional], it's optional
			Treatment = AttributeCache<OptionalAttribute>.HasAttribute(property) ?
						TypeTreatment.Optional

			// it doesn't matter if it has [Required] or doesn't, it's required.
						: TypeTreatment.Required;
			
			PropertyInfo = property;

			Flatten = AttributeCache<FlattenAttribute>.HasAttribute(property);

			Type = property.PropertyType;
			TypeHashcode = Type.GetHashCode();
			
			_propSetRaw = PropertyInfo.GetSetMethodByExpression();
			_propGetRaw = PropertyInfo.GetGetMethod().ILWrap();
		}

		public uint Position;
		internal int PositionInt;

		public TypeTreatment Treatment;

		public PropertyInfo PropertyInfo;

		public bool Flatten;

		public Type Type;
		public int TypeHashcode;

		private readonly Action<object, object> _propSetRaw;
		private readonly ILFunc _propGetRaw;

		public object Get(object instance)
			=> _propGetRaw(instance, default);

		public void Set(object instance, object value)
			=> _propSetRaw(instance, value);
	}
}