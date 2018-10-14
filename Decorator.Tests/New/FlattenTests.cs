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

		[Theory, Trait("Category", nameof(FlattenTests))]
		[MemberData(nameof(GetValuesDeserializeProperlyDeserializationValues))]
		public void ValuesDeserializeProperlyDeserialization(VerySimple msg, BaseMessage bm)
		{
			Deserializer.TryDeserializeItem<VerySimple>(bm, out var compare);

			Assert.Equal(msg.Data, compare.Data);
			Assert.Equal(msg.DemoData.SomeData, compare.DemoData.SomeData);
			Assert.Equal(msg.DemoData.IntegerData, compare.DemoData.IntegerData);
		}

		public static IEnumerable<object[]> GetValuesDeserializeProperlyDeserializationValues()
		{
			//TODO: consts
			yield return new object[] { new VerySimple {
				 Data = "weee",
				 DemoData = new DemoMessage {
					 IntegerData = 1337,
					 SomeData = "s o m e d a t a a a"
				 }
			}, new BasicMessage("simple", "weee", "s o m e d a t a a a", 1337)};
		}

		[Message("simple")]
		public class MultipleDataPieces
		{
			[Position(0), Required]
			public string Data { get; set; }

			[Position(1), Required, Flatten]
			public DemoMessage DemoData { get; set; }

			[Position(2), Required]
			public int DataInfo { get; set; }

			[Position(3), Required, Flatten]
			public DemoMessage MoreDemoData { get; set; }
		}

		[Theory, Trait("Category", nameof(FlattenTests))]
		[MemberData(nameof(GetMultipleDataPiecesDeserializationValues))]
		public void MultipleDataPiecesDeserialization(MessageInfo messageInfo)
			=> Assert.Equal(messageInfo.ExpectedResult, Deserializer.TryDeserializeItem<MultipleDataPieces>(new BasicMessage(messageInfo.Type, messageInfo.Arguments), out _));

		public static IEnumerable<object[]> GetMultipleDataPiecesDeserializationValues()
		{
			yield return new object[] { new MessageInfo(true, "simple", new object[] { "data", "somedata", 14122, 42, "moredata", 13122 }) };
			yield return new object[] { new MessageInfo(true, "simple", new object[] { "data", "somedata", "optional int, remember?", 42, "required string", null }) };
			yield return new object[] { new MessageInfo(true, "simple", new object[] { "data", "somedata", 42, 92, "", 1 }) };
			yield return new object[] { new MessageInfo(false, "simple", new object[] { "data", "", 89, null, "", 4 }) };
			yield return new object[] { new MessageInfo(false, "simple", new object[] { 2, "", 1, 1, "", 1 }) };
		}

		[Message("simple")]
		public class SkippingPlaces
		{
			[Position(0), Required, Flatten]
			public DemoMessage DemoData { get; set; }

			[Position(3), Required, Flatten]
			public DemoMessage MoreDemoData { get; set; }
		}

		[Theory, Trait("Category", nameof(FlattenTests))]
		[MemberData(nameof(GetSkippingPlacesDeserializationValues))]
		public void SkippingPlacesDeserialization(MessageInfo messageInfo)
			=> Assert.Equal(messageInfo.ExpectedResult, Deserializer.TryDeserializeItem<SkippingPlaces>(new BasicMessage(messageInfo.Type, messageInfo.Arguments), out _));

		public static IEnumerable<object[]> GetSkippingPlacesDeserializationValues()
		{
			yield return new object[] { new MessageInfo(true, "simple", new object[] { "data", 1234, null, null, "moredata", 5678 }) };
			yield return new object[] { new MessageInfo(true, "simple", new object[] { "data", 1234, 55, 66, "moredata", 5678 }) };
			yield return new object[] { new MessageInfo(false, "simple", new object[] { "data", 1234, 55, 66, null, 5678 }) };
			yield return new object[] { new MessageInfo(false, "simple", new object[] { null, 1234, 55, 66, "moredata", 5678 }) };
			yield return new object[] { new MessageInfo(false, "simple", new object[] { "data", 1234, "moredata", 5678 }) };
		}

		public class ArrayInvokeHandler
		{
			public bool Invoked { get; set; }

			[DeserializedHandler]
			public void AcceptArray(VerySimple[] items)
			{
				Invoked = true;

				Assert.Equal(3, items.Length);

				foreach (var i in items)
				{
					Assert.Equal(i.Data + "_", i.DemoData.SomeData);
					Assert.Equal(Convert.ToInt32(i.Data), i.DemoData.IntegerData);
				}
			}
		}

		[Theory, Trait("Category", nameof(FlattenTests))]
		[MemberData(nameof(GetHandlerDeserializationValues))]
		public void HandlerDeserialization(bool expected, BaseMessage msg)
		{
			var instance = new ArrayInvokeHandler();

			Deserializer<ArrayInvokeHandler>.InvokeMethodFromMessage(instance, msg);

			Assert.Equal(expected, instance.Invoked);
		}

		public static IEnumerable<object[]> GetHandlerDeserializationValues()
		{
			yield return new object[] { true, new BasicMessage("simple", "1", "1_", 1, "9", "9_", 9, "3", "3_", 3) };
			yield return new object[] { false, new BasicMessage("notsimple", "1", "1_", 1, "9", "9_", 9, "3", "3_", 3) };
			yield return new object[] { false, new BasicMessage("simple", "1", "1_", 1, "9", "9_", 9, "3", "3_", 3, "lol") };
			yield return new object[] { false, new BasicMessage("simple", "1", "1_", 1, "9", "9_", 9, "3", "3_") };
		}

		//TODO: Flatten Arrays Tests

		/*
		[Message("fsat")]
		public class FlattenSimpleArraysTest
		{
			[Position(0), Flatten]
			public string[] SomeStringData { get; set; }

			public override bool Equals(object obj)
			{
				return obj is FlattenSimpleArraysTest fsat &&
						fsat.SomeStringData.Equals(this.SomeStringData);
			}
		}
		
		[Theory, Trait("Category", nameof(FlattenTests))]
		[MemberData(nameof(GetFlattenSimpleArrayDeserializationValues))]
		public void FlattenSimpleArrayDeserialization(bool expected, BaseMessage msg, FlattenSimpleArraysTest fsat)
		{
			Assert.Equal(expected, Deserializer.TryDeserializeItem<FlattenSimpleArraysTest>(msg, out var res));

			if (expected)
				Assert.Equal(res, fsat);
		}

		public static IEnumerable<object[]> GetFlattenSimpleArrayDeserializationValues()
		{
			yield return new object[] { true, new BasicMessage("fsat", 3, "a", "b", "c"), new FlattenSimpleArraysTest { SomeStringData = new[] { "a", "b", "c" } } };
			yield return new object[] { true, new BasicMessage("fsat", 4, "a", "b", "c", "d"), new FlattenSimpleArraysTest { SomeStringData = new[] { "a", "b", "c", "d" } } };
			yield return new object[] { true, new BasicMessage("fsat", 3, "232", "ee", "GgG"), new FlattenSimpleArraysTest { SomeStringData = new[] { "232", "ee", "GgG" } } };
			yield return new object[] { true, new BasicMessage("fsat", 1, ""), new FlattenSimpleArraysTest { SomeStringData = new[] { "" } } };
			yield return new object[] { true, new BasicMessage("fsat", 0), new FlattenSimpleArraysTest { SomeStringData = new string[] { } } };
		}

		[Message("fsat")]
		public class FlattenComplexArraysTest
		{
			[Position(0), Flatten]
			public DemoMessage[] SomeStringData { get; set; }

			public override bool Equals(object obj)
			{
				return obj is FlattenComplexArraysTest fsat &&
						fsat.SomeStringData.Equals(this.SomeStringData);
			}
		}

		[Theory, Trait("Category", nameof(FlattenTests))]
		[MemberData(nameof(GetFlattenComplexArrayDeserializationValues))]
		public void FlattenComplexArrayDeserialization(bool expected, BaseMessage msg, FlattenComplexArraysTest fsat)
		{
			Assert.Equal(expected, Deserializer.TryDeserializeItem<FlattenComplexArraysTest>(msg, out var res));

			if (expected)
				Assert.Equal(res, fsat);
		}

		public static IEnumerable<object[]> GetFlattenComplexArrayDeserializationValues()
		{
			yield return new object[] { true, new BasicMessage("fsat", 3, "message", 1234, "more", 4567, "8778", null), new FlattenComplexArraysTest { SomeStringData = new DemoMessage[] {
				new DemoMessage { SomeData= "message", IntegerData = 1234 },
				new DemoMessage { SomeData= "more", IntegerData = 4567 },
				new DemoMessage { SomeData= "8778", IntegerData = default }
			} } };
			yield return new object[] { true, new BasicMessage("fsat", 0), new FlattenComplexArraysTest { SomeStringData = new DemoMessage[] { } } };
		}
		*/
	}
}
