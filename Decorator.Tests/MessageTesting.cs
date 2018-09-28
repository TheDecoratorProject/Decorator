using Xunit;

namespace Decorator.Tests {

	public class MessageTesting {

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", nameof(MessageTesting))]
		public void GetsArgument() {
			var at = 2;

			var msg = new BasicMessage("test", 1, at, 3);

			Assert.Equal(at, msg[1]);
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", nameof(MessageTesting))]
		public void Hashcode() {
			var msg = new BasicMessage("test", 1);

			// make sure it doesn't throw

			msg.GetHashCode();
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", nameof(MessageTesting))]
		public void DoesDoEqualsCorrectly() {
			var msg = new BasicMessage("test", 992, 0b1010);

			var msg2 = new BasicMessage("test", 992, 0b1010);

			Assert.True(msg.Equals(msg2));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", nameof(MessageTesting))]
		public void DoesDoesntEqualsCorrectly1() {
			var msg = new BasicMessage("test", 92, 0b1010);

			var msg2 = new BasicMessage("test", 992, 0b1010);

			Assert.False(msg.Equals(msg2));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", nameof(MessageTesting))]
		public void InequalByNull() {
			var msg = new BasicMessage("test", 92);

			Assert.False(msg.Equals(null));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", nameof(MessageTesting))]
		public void InequalByArgumentsNull() {
			var msg = new BasicMessage("test", 92);

			Assert.False(msg.Equals(new BasicMessage("test", null)));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", nameof(MessageTesting))]
		public void EqualsByReference() {
			var msg = new BasicMessage("test", 92, 0b1010);
			var msg2 = msg;

			Assert.True(msg.Equals(msg2));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", nameof(MessageTesting))]
		public void InequalByType()
		{
			var msg = new BasicMessage("test", 1);
			var msg2 = new BasicMessage("tot", 1);

			Assert.False(msg.Equals(msg2));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", nameof(MessageTesting))]
		public void InequalByParameterAmount()
		{
			var msg = new BasicMessage("test", 1);
			var msg2 = new BasicMessage("test", 1, 2);

			Assert.False(msg.Equals(msg2));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", nameof(MessageTesting))]
		public void InequalByNullArgs()
		{
			var msg = new BasicMessage("test", null);
			var msg2 = new BasicMessage("test", 1);

			Assert.False(msg.Equals(msg2));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", nameof(MessageTesting))]
		public void InequalByOthersNullArgs()
		{
			var msg = new BasicMessage("test", 1);
			var msg2 = new BasicMessage("test", null);

			Assert.False(msg.Equals(msg2));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", nameof(MessageTesting))]
		public void FalseGenericObject() {
			var msg = new BasicMessage("test", 992, 0b1010);

			Assert.False(msg.Equals(""));
		}

		[Fact, Trait("Project", "Decorator.Tests")]
		[Trait("Category", nameof(MessageTesting))]
		public void TrueExecutesBaseEquals()
		{
			var msg = new BasicMessage("test", 992, 0b1010);
			var msg2 = new BasicMessage("test", 992, 0b1010);

			Assert.True(msg.Equals((object)msg2));
		}
	}
}