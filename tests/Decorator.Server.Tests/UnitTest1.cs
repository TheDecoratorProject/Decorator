using Decorator.Attributes;
using Decorator.Modules;

using FluentAssertions;

using Xunit;

namespace Decorator.Server.Tests
{
	[Message("a")]
	public class TestMessageOneMessage
	{
		[Position(0), Required]
		public string TestName { get; set; }

		[Position(1), Required]
		public string TestValue { get; set; }
	}

	[Message("b")]
	public class TestMessageTwoMessage
	{
		[Position(0), Required]
		public int TestName { get; set; }

		[Position(1), Required]
		public int TestValue { get; set; }
	}

	public class Test
	{
		private bool _invokedOne;
		private bool _invokedTwo;

		[Fact]
		public void CanInvokeProperly()
		{
			MessageInvoker<Test>.Invoke(this, new BaseMessage("a", new object[] { "abcd", "efgh" }))
				.Should().BeNull();

			_invokedOne.Should().BeTrue();

			((bool)(MessageInvoker<Test>.Invoke(this, new BaseMessage("b", new object[] { 1234, 5678 }))))
				.Should().BeTrue();

			_invokedTwo.Should().BeTrue();
		}

		[MessageHandler]
		public void ExampleHandlerOne(TestMessageOneMessage tmo)
		{
			_invokedOne = true;

			tmo.TestName.Should().Be("abcd");
			tmo.TestValue.Should().Be("efgh");
		}

		[MessageHandler]
		public bool ExampleHandlerTwo(TestMessageTwoMessage tmo)
		{
			_invokedTwo = true;

			tmo.TestName.Should().Be(1234);
			tmo.TestValue.Should().Be(5678);

			return true;
		}
	}
}