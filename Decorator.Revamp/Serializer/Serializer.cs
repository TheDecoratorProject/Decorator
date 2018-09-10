using Decorator.Attributes;
using Decorator.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
    public static class Serializer<TClass>
		where TClass : class
    {
		static Serializer() {
			if (!typeof(TClass).HasAttribute<MessageAttribute>(out var _)) throw new DecoratorException("lmao u should fhave an atrib");
		}

		public static BaseMessage Serialize(TClass itm) {
			var inst = ReflectionHelper.CreateGen<TClass>()();

			var def = Deserializer.TypeManager.GetDefinitionFor<TClass>();

			var data = new object[def.MaxCount];

			foreach(var i in def.Properties)
				data[(int)i.Position] = i.Get(itm);

			return new BasicMessage(def.Type, data);
		}
    }
}
