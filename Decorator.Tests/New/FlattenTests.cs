using Decorator.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Decorator.Tests.New
{
	public class FlattenTests
	{
		[Message("demo")]
		public class DemoMessage
		{
			[Position(0), Required]
			public string SomeData { get; set; }

			[Position(1), Optional]
			public int IntegerData { get; set; }
		}

		[Message("simple")]
		public class VerySimple
		{
			[Position(0), Required]
			public string Data { get; set; }

			[Position(1), Required, Flatten]
			public DemoMessage DemoData { get; set; }
		}

		[Theory, Trait("Category", nameof(FlattenTests))]
		[MemberData(nameof(GetVerySimpleDeserializationValues))]
		public void VerySimpleDeserialization(MessageInfo messageInfo)
			=> Assert.Equal(messageInfo.ExpectedResult, Deserializer.TryDeserializeItem<VerySimple>(new BasicMessage(messageInfo.Type, messageInfo.Arguments), out _));

		public static IEnumerable<object[]> GetVerySimpleDeserializationValues()
		{
			yield return new object[] { new MessageInfo(true, "simple", new object[] { "data", "somedata", 14122 }) };
			yield return new object[] { new MessageInfo(true, "simple", new object[] { "data", "somedata", "optional int, remember?" }) };
			yield return new object[] { new MessageInfo(true, "simple", new object[] { "data", "somedata", null }) };
			yield return new object[] { new MessageInfo(false, "simple", new object[] { "data", 4, 14122 }) };
			yield return new object[] { new MessageInfo(false, "simple", new object[] { 2, "somedata", 42, }) };
		}
	}
}
