using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator.Tests
{
    public static class Setup {
		public static (Deserializer<HandlerClass> Deserializer, HandlerClass Instance) GetSetup() {
			var des = new Deserializer<HandlerClass>();
			var inst = new HandlerClass();

			return (des, inst);
		}

		public static Message TooShort => new MessageImplementation(
					"test",
					"too short"
				);

		public static Message TooLong => new MessageImplementation(
					"test",
					"too long",
					1337,
					3f
				);

		public static Message InvalidBase => new MessageImplementation(
					"invalidBase",
					"just right",
					1337
				);

		public static Message IncorrectTypes => new MessageImplementation(
					"test",
					1337,
					"ohoopsies"
				);

		public static Message NullValues => new MessageImplementation(
					"test",
					null,
					null
				);

		public static Message NullType => new MessageImplementation(
					null,
					"just right",
					1337
				);

		public static Message AllNull => new MessageImplementation(
					null,
					null,
					null
				);

		public static Message Correct => new MessageImplementation(
					"test",
					"just right",
					1337
				);

		public static Message OnlyNullType => new MessageImplementation(null);
	}
}
