namespace Decorator
{
	/// <summary>An implementation of BaseMessage that meets the minimum bar.</summary>
	/// <seealso cref="Decorator.BaseMessage" />
	public class BasicMessage : BaseMessage
	{
		public BasicMessage(string type, params object[] args)
		{
			Type = type;
			Arguments = args;

			_argLen = (args ?? new object[0]).Length;
		}

		private readonly int _argLen;
		public override int Count => _argLen;

		public override string Type { get; }
		public override object[] Arguments { get; }
	}
}