using Decorator.Attributes;

using Xunit;

namespace Decorator.Tests {

	public interface ITestable {

		void AssertType(Message msg);

		void AssertArgs(object[] args);
	}

	[Message(NonNullType.TypeName)]
	public class NonNullType : ITestable {
		public const string TypeName = "serialization";

		[Position(0)]
		[Optional]
		public string Value { get; set; }

		public void AssertType(Message msg) => Assert.Equal(msg.Type, TypeName);

		public void AssertArgs(object[] args) {
			Assert.Equal(args.Length, 1);
			Assert.Equal(args[0], this.Value);
		}

		public static ITestable GetInstance(object data)
			=> new NonNullType {
				Value = (string)data
			};
	}

	[Message(null)]
	public class NullType : ITestable {

		public void AssertType(Message msg) => Assert.Null(msg.Type);

		[Position(0)]
		[Optional]
		public object Value { get; set; }

		public void AssertArgs(object[] args) {
			Assert.Equal(args.Length, 1);
			Assert.Equal(args[0], this.Value);
		}

		public static ITestable GetInstance(object data)
			=> new NullType {
				Value = data
			};
	}
}