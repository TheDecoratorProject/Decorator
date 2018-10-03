using Decorator.Attributes;

using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;

namespace Decorator.Tests
{
	public class MessageInfo : IXunitSerializable
	{
		public MessageInfo()
		{
		}

		public MessageInfo(bool result, string type, object[] args)
		{
			ExpectedResult = result;
			Type = type;
			Arguments = args;
		}

		public bool ExpectedResult { get; set; }
		public string Type { get; set; }
		public object[] Arguments { get; set; }

		public void Deserialize(IXunitSerializationInfo info)
		{
			ExpectedResult = info.GetValue<bool>(nameof(ExpectedResult));
			Type = info.GetValue<string>(nameof(Type));
			Arguments = info.GetValue<object[]>(nameof(Arguments));
		}

		public void Serialize(IXunitSerializationInfo info)
		{
			info.AddValue(nameof(ExpectedResult), ExpectedResult);
			info.AddValue(nameof(Type), Type);
			info.AddValue(nameof(Arguments), Arguments);
		}

		public static MessageInfo Make() => new MessageInfo();
	}

	public class TestDeserializeChecking
	{
		#region TestMessage

		public const string TESTMESSAGE_TYPE = "test";

		[Message(TESTMESSAGE_TYPE)]
		public class TestMessage
		{
			[Position(0), Required]
			public string StringValue { get; set; }

			[Position(1), Required]
			public int IntegerValue { get; set; }
		}

		[Theory, Trait("Category", nameof(TestDeserializeChecking))]
		[MemberData(nameof(GetTestMessageDeserializationValues))]
		public void TestMessageDeserialization(MessageInfo messageInfo)
			=> Assert.Equal(messageInfo.ExpectedResult, Deserializer.TryDeserializeItem<TestMessage>(new BasicMessage(messageInfo.Type, messageInfo.Arguments), out _));

		public static IEnumerable<object[]> GetTestMessageDeserializationValues()
		{
			yield return new object[] { new MessageInfo(false, TESTMESSAGE_TYPE, new object[] { }) };
			yield return new object[] { new MessageInfo(false, TESTMESSAGE_TYPE, new object[] { "too short" }) };
			yield return new object[] { new MessageInfo(true, TESTMESSAGE_TYPE, new object[] { "too long", 1, "..." }) };
			yield return new object[] { new MessageInfo(false, TESTMESSAGE_TYPE, new object[] { 1, "incorrect types" }) };
			yield return new object[] { new MessageInfo(false, TESTMESSAGE_TYPE, new object[] { null, null }) };
			yield return new object[] { new MessageInfo(false, TESTMESSAGE_TYPE, new object[] { null, 100 }) };
			yield return new object[] { new MessageInfo(false, "incorrect-base", new object[] { "(in)valid message", 1234 }) };
			yield return new object[] { new MessageInfo(false, null, new object[] { "(in)valid message", 1234 }) };
			yield return new object[] { new MessageInfo(true, TESTMESSAGE_TYPE, new object[] { "valid message", 1234 }) };
		}

		#endregion TestMessage

		#region NullMessage

		[Message(null)]
		public class NullMessage
		{
		}

		[Theory, Trait("Category", nameof(TestDeserializeChecking))]
		[MemberData(nameof(GetNullMessageDeserializationValues))]
		public void NullMessageDeserialization(MessageInfo msgInfo)
			=> Assert.Equal(msgInfo.ExpectedResult, Deserializer.TryDeserializeItem<NullMessage>(new BasicMessage(msgInfo.Type, msgInfo.Arguments), out _));

		public static IEnumerable<object[]> GetNullMessageDeserializationValues()
		{
			yield return new object[] { new MessageInfo(false, "", new object[] { "" }) };
			yield return new object[] { new MessageInfo(false, "", new object[] { null }) };
			yield return new object[] { new MessageInfo(false, "", new object[] { }) };
			yield return new object[] { new MessageInfo(false, "", null) };
			yield return new object[] { new MessageInfo(true, null, new object[] { "" }) };
			yield return new object[] { new MessageInfo(true, null, new object[] { null }) };
			yield return new object[] { new MessageInfo(true, null, new object[] { }) };
			yield return new object[] { new MessageInfo(true, null, null) };
		}

		#endregion NullMessage

		#region OptionalMessage

		public const string OPTIONAL_TYPE = "opt";

		[Message(OPTIONAL_TYPE)]
		public class OptionalMessage
		{
			[Position(0), Optional]
			public int IntegerValue { get; set; }
		}

		[Theory, Trait("Category", nameof(TestDeserializeChecking))]
		[MemberData(nameof(GetOptionalMessageDeserializationValues))]
		public void OptionalMessageDeserialization(MessageInfo messageInfo)
			=> Assert.Equal(messageInfo.ExpectedResult, Deserializer.TryDeserializeItem<OptionalMessage>(new BasicMessage(messageInfo.Type, messageInfo.Arguments), out _));

		public static IEnumerable<object[]> GetOptionalMessageDeserializationValues()
		{
			yield return new object[] { new MessageInfo(true, OPTIONAL_TYPE, new object[] { }) };
			yield return new object[] { new MessageInfo(true, OPTIONAL_TYPE, new object[] { null, null }) };
			yield return new object[] { new MessageInfo(true, OPTIONAL_TYPE, new object[] { 1010, 1100 }) };
			yield return new object[] { new MessageInfo(false, $"n{OPTIONAL_TYPE}", new object[] { 1234 }) };
			yield return new object[] { new MessageInfo(true, OPTIONAL_TYPE, new object[] { null }) };
			yield return new object[] { new MessageInfo(true, OPTIONAL_TYPE, new object[] { "invalid type" }) };
			yield return new object[] { new MessageInfo(true, OPTIONAL_TYPE, new object[] { 1234 }) };
			yield return new object[] { new MessageInfo(true, OPTIONAL_TYPE, new object[] { 3f }) };
		}

		#endregion OptionalMessage

		#region OptionalsShouldBeSkipped
		public const string OSBS_TYPE = "osbs";

		[Message(OSBS_TYPE)]
		public class OptionalsShouldBeSkipped
		{
			[Position(0), Required]
			public string RequiredString { get; set; }

			[Position(1), Optional]
			public int OptionalValue { get; set; }

			[Position(2), Required]
			public string AnotherRequiredString { get; set; }

			[Position(3), Optional]
			public int UhgAnother { get; set; }

			[Position(4), Optional]
			public int YetAnotherOptionalValue { get; set; }

			[Position(5), Required]
			public string AlasAnotherRequiredString { get; set; }
		}

		[Theory, Trait("Category", nameof(TestDeserializeChecking))]
		[MemberData(nameof(OptionalsShouldBeSkippedDeserializationValues))]
		public void OptionalsShouldBeSkippedDeserialization(MessageInfo messageInfo)
			=> Assert.Equal(messageInfo.ExpectedResult, Deserializer.TryDeserializeItem<OptionalsShouldBeSkipped>(new BasicMessage(messageInfo.Type, messageInfo.Arguments), out _));

		public static IEnumerable<object[]> OptionalsShouldBeSkippedDeserializationValues()
		{
			yield return new object[] { new MessageInfo(false, OSBS_TYPE, new object[] { null, null, null, null, null, null }) };
			yield return new object[] { new MessageInfo(true, OSBS_TYPE, new object[] { "", "", "", "", "", "" }) };
			yield return new object[] { new MessageInfo(true, OSBS_TYPE, new object[] { "", 1, "", 1, 1, "" }) };
			yield return new object[] { new MessageInfo(false, OSBS_TYPE, new object[] { 1, 1, "", 1, 1, "" }) };
			yield return new object[] { new MessageInfo(true, OSBS_TYPE, new object[] { "", "", "", 1, 1, "" }) };
		}

		#endregion
	}
}