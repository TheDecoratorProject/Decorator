using System.Reflection;

using Xunit;

namespace Decorator.Tests {

	public class Serialization {

		private void TestTestable<T>(T test)
			where T : ITestable {
			var msg = Serializer.Serialize(test);

			test.AssertType(msg);
			Assert.True(msg.Args != null, $"Null arguments.");
			test.AssertArgs(msg.Args);
		}

		[Fact(DisplayName = "Type:      Null      Value:    Non-Null")]
		public void _1()
			=> TestTestable(NullType.GetInstance(1234));

		[Fact(DisplayName = "Type:      Null      Value:    Null")]
		public void _2()
			=> TestTestable(NullType.GetInstance(null));

		[Fact(DisplayName = "Type:      Non-Null  Value:    Null")]
		public void _3()
			=> TestTestable(NonNullType.GetInstance(null));

		[Fact(DisplayName = "Type:      Non-Null  Value:    Non-Null")]
		public void _4()
			=> TestTestable(NonNullType.GetInstance("lorem ipsum"));

		[Fact(DisplayName = "SerializeNull")]
		public void _5()
			=> Assert.Throws<CustomAttributeFormatException>(() => Serializer.Serialize<ITestable>((ITestable)null));
	}
}