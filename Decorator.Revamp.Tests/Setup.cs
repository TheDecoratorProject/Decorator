namespace Decorator.Tests {

	public static class Setup {

		public static (Deserializer<HandlerClass> Deserializer, HandlerClass Instance) GetSetup() {
			var des = new Deserializer<HandlerClass>();
			var inst = new HandlerClass();

			return (des, inst);
		}

		public static BaseMessage TooShort => new BasicMessage(
					"test",
					"too short"
				);

		public static BaseMessage TooLong => new BasicMessage(
					"test",
					"too long",
					1337,
					3f
				);

		public static BaseMessage InvalidBase => new BasicMessage(
					"invalidBase",
					"just right",
					1337
				);

		public static BaseMessage IncorrectTypes => new BasicMessage(
					"test",
					1337,
					"ohoopsies"
				);

		public static BaseMessage NullValues => new BasicMessage(
					"test",
					null,
					null
				);

		public static BaseMessage NullType => new BasicMessage(
					null,
					"just right",
					1337
				);

		public static BaseMessage AllNull => new BasicMessage(
					null,
					null,
					null
				);

		public static BaseMessage Correct => new BasicMessage(
					"test",
					"just right",
					1337
				);

		public static BaseMessage OnlyNullType => new BasicMessage(null);
	}
}