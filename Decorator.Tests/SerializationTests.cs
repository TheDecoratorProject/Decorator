using Decorator.Attributes;
using Decorator.Modules;

using FluentAssertions;

using System;
using System.Collections.Generic;

using Xunit;

namespace Decorator.Tests
{
	public class SerializationTests
	{
		public class SerializationTestsIgnoredAttributeBase
		{
			[Position(0), Ignored]
			public string MyReferenceType { get; set; }

			[Position(1), Ignored]
			public int MyValueType { get; set; }
		}

		public class SerializationTestsRequiredAttributeBase
		{
			[Position(0), Required]
			public string MyReferenceType { get; set; }

			[Position(1), Required]
			public int MyValueType { get; set; }
		}

		public class SerializationTestsOptionalAttributeBase
		{
			[Position(0), Optional]
			public string MyReferenceType { get; set; }

			[Position(1), Optional]
			public int MyValueType { get; set; }
		}

		public class SerializationTestsArrayAttributeBase
		{
			[Position(0), Array]
			public string[] MyReferenceTypes { get; set; }

			[Position(1), Array]
			public int[] MyValueTypes { get; set; }
		}

		public class SerializationTestsFlattenAttributeBase
		{
			[Position(0), Flatten]
			public SerializationTestsRequiredAttributeBase RequiredDecorable { get; set; }

			[Position(1), Flatten]
			public SerializationTestsOptionalAttributeBase OptionalDecorable { get; set; }
		}

		public class SerializationTestsFlattenArrayAttributeBase
		{
			[Position(0), FlattenArray]
			public SerializationTestsRequiredAttributeBase[] RequiredDecorable { get; set; }

			[Position(1), FlattenArray]
			public SerializationTestsOptionalAttributeBase[] OptionalDecorable { get; set; }
		}

		[Theory]
		[MemberData(nameof(IgnoredData))]
		public void Ignored(SerializationTestsIgnoredAttributeBase test, object[] serializedData)
		{
			DConverter<SerializationTestsIgnoredAttributeBase>.Serialize(test)
				.Should().BeEquivalentTo(serializedData);
		}

		public static IEnumerable<object[]> IgnoredData()
		{
			yield return new object[]
			{
				new SerializationTestsIgnoredAttributeBase
				{
					MyReferenceType = "abcd",
					MyValueType = 4567,
				},
				new object[]
				{
					null,
					null
				}
			};
			yield return new object[]
			{
				new SerializationTestsIgnoredAttributeBase(),
				new object[]
				{
					null,
					null
				}
			};
		}

		[Theory]
		[MemberData(nameof(RequiredData))]
		public void Required(SerializationTestsRequiredAttributeBase test, object[] serializedData)
		{
			DConverter<SerializationTestsRequiredAttributeBase>.Serialize(test)
				.Should().BeEquivalentTo(serializedData);
		}

		public static IEnumerable<object[]> RequiredData()
		{
			yield return new object[]
				{
					new SerializationTestsRequiredAttributeBase
					{
						MyReferenceType = "abcd",
						MyValueType = 4567,
					},
					new object[]
					{
						"abcd",
						4567
					}
				};
			yield return new object[]
				{
					new SerializationTestsRequiredAttributeBase
					{
						MyReferenceType = null,
						MyValueType = 4567,
					},
					new object[]
					{
						null,
						4567
					}
				};
		}

		[Theory]
		[MemberData(nameof(OptionalData))]
		public void Optional(SerializationTestsOptionalAttributeBase test, object[] serializedData)
		{
			DConverter<SerializationTestsOptionalAttributeBase>.Serialize(test)
				.Should().BeEquivalentTo(serializedData);
		}

		public static IEnumerable<object[]> OptionalData()
		{
			yield return new object[]
				{
					new SerializationTestsOptionalAttributeBase
					{
						MyReferenceType = "abcd",
						MyValueType = 4567,
					},
					new object[]
					{
						"abcd",
						4567
					}
				};
			yield return new object[]
				{
					new SerializationTestsOptionalAttributeBase(),
					new object[]
					{
						null,
						0
					}
				};
		}

		[Theory]
		[MemberData(nameof(ArrayData))]
		public void Array(SerializationTestsArrayAttributeBase test, object[] serializedData)
		{
			DConverter<SerializationTestsArrayAttributeBase>.Serialize(test)
				.Should().BeEquivalentTo(serializedData);
		}

		public static IEnumerable<object[]> ArrayData()
		{
			yield return new object[]
				{
					new SerializationTestsArrayAttributeBase
					{
						MyReferenceTypes = new [] { "a", "b", "c", "d" },
						MyValueTypes = new [] { 1, 2, 3, 4, 5 }
					},
					new object[]
					{
						4, "a", "b", "c", "d",
						5, 1, 2, 3, 4, 5
					}
				};
			yield return new object[]
				{
					new SerializationTestsArrayAttributeBase
					{
						MyReferenceTypes = new string[] { },
						MyValueTypes = new [] { 1, 2, 3, 4, 5 }
					},
					new object[]
					{
						0,
						5, 1, 2, 3, 4, 5
					}
				};
			yield return new object[]
				{
					new SerializationTestsArrayAttributeBase
					{
						MyReferenceTypes = new [] { "a", "b", "c", "d" },
						MyValueTypes = new int[] { }
					},
					new object[]
					{
						4, "a", "b", "c", "d",
						0
					}
				};
			yield return new object[]
				{
					new SerializationTestsArrayAttributeBase
					{
						MyReferenceTypes = new string[] { },
						MyValueTypes = new int[] { }
					},
					new object[]
					{
						0,
						0
					}
				};
		}

		[Theory]
		[MemberData(nameof(FlattenData))]
		public void Flatten(SerializationTestsFlattenAttributeBase test, object[] serializedData)
		{
			DConverter<SerializationTestsFlattenAttributeBase>.Serialize(test)
				.Should().BeEquivalentTo(serializedData);
		}

		public static IEnumerable<object[]> FlattenData()
		{
			yield return new object[]
				{
					new SerializationTestsFlattenAttributeBase
					{
						RequiredDecorable = new SerializationTestsRequiredAttributeBase
						{
							MyReferenceType = "string",
							MyValueType = 1234,
						},
						OptionalDecorable = new SerializationTestsOptionalAttributeBase
						{
							MyReferenceType = null,
							MyValueType = 0,
						}
					},
					new object[]
					{
						"string",
						1234,

						null,
						0
					}
				};
			yield return new object[]
				{
					new SerializationTestsFlattenAttributeBase
					{
						//TODO: not setting these causes an exception.
						// Maybe we could figure out a way to detonate that "hey, these aren't null" or something?
						//
						RequiredDecorable = new SerializationTestsRequiredAttributeBase(),
						OptionalDecorable = new SerializationTestsOptionalAttributeBase(),
					},
					new object[]
					{
						null,
						0,

						null,
						0
					}
				};
		}

		[Theory]
		[MemberData(nameof(FlattenArrayData))]
		public void FlattenArray(SerializationTestsFlattenArrayAttributeBase test, object[] serializedData)
		{
			DConverter<SerializationTestsFlattenArrayAttributeBase>.Serialize(test)
				.Should().BeEquivalentTo(serializedData);
		}

		public static IEnumerable<object[]> FlattenArrayData()
		{
			yield return new object[]
				{
					new SerializationTestsFlattenArrayAttributeBase
					{
						RequiredDecorable = new SerializationTestsRequiredAttributeBase[] { },
						OptionalDecorable = new SerializationTestsOptionalAttributeBase[] { },
					},
					new object[]
					{
						0,
						0
					}
				};
			yield return new object[]
				{
					new SerializationTestsFlattenArrayAttributeBase
					{
						RequiredDecorable = new []
						{
							new SerializationTestsRequiredAttributeBase()
						},
						OptionalDecorable = new []
						{
							new SerializationTestsOptionalAttributeBase()
						},
					},
					new object[]
					{
						1,
						null, 0,
						1,
						null, 0
					}
				};
		}
	}
}