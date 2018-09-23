namespace Decorator {

	/// <summary>An implementation of BaseMessage that meets the minimum bar.</summary>
	/// <seealso cref="Decorator.BaseMessage" />
	public class BasicMessage : BaseMessage {

		public BasicMessage(string type, params object[] args) {
			this.Type = type;
			this.Arguments = args;
		}

		public override string Type { get; }
		public override object[] Arguments { get; }
	}
}