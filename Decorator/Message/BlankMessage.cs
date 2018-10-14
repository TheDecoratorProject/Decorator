namespace Decorator
{
	/// <summary>An implementation of BaseMessage that meets the minimum bar, and doesn't force you to specify a type.</summary>
	/// <seealso cref="Decorator.BaseMessage" />
	public class BlankMessage : BaseMessage
	{
		public BlankMessage(params object[] args)
		{
			Type = null;
			Arguments = args;

			_argLen = (args ?? new object[0]).Length;
		}

		private readonly int _argLen;
		public override int Count => _argLen;

		public override string Type { get; }
		public override object[] Arguments { get; }
	}
}