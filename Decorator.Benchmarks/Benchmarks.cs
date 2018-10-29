using BenchmarkDotNet.Attributes;
using Deserialiser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decorator.Benchmarks
{
	[Deserialisable]
	[ProtocolMessage.Message(" ")]
	public class TestClass : IDecorable
	{
		[ProtocolMessage.Position(0), ProtocolMessage.Required]
		[Position(0), Required]
		[Order(0)]
		public string String { get; set; }

		[ProtocolMessage.Position(1), ProtocolMessage.Required]
		[Position(1), Required]
		[Order(1)]
		public int Int { get; set; }

		[ProtocolMessage.Position(2), ProtocolMessage.Required]
		[Position(2), Required]
		[Order(2)]
		public int[] SomeIntegers { get; set; }

		[ProtocolMessage.Position(3), ProtocolMessage.Required]
		[Position(3), Required]
		[Order(3)]
		public long LongInt { get; set; }

		[ProtocolMessage.Position(4), ProtocolMessage.Required]
		[Position(4), Required]
		[Order(4)]
		public ulong UltraLongInt { get; set; }

		[ProtocolMessage.Position(5), ProtocolMessage.Required]
		[Position(5), Required]
		[Order(5)]
		public byte Identifier { get; set; }

		[ProtocolMessage.Position(6), ProtocolMessage.Required]
		[Position(6), Required]
		[Order(6)]
		public sbyte SuperUselessByte { get; set; }
	}

	public class Benchmarks
	{
		private TestClass _testClass;
		private object[] _data;
		private ProtocolMessage.ProtocolMessageManager _pm;

		public Benchmarks()
		{
			_testClass = new TestClass
			{
				String = "Hello, World!",
				Int = 1234,
				SomeIntegers = new int[]
				{
					1, 2, 8, 9, 20, 15
				},
				LongInt = 1106300L,
				UltraLongInt = 11063002uL,
				Identifier = 0xFF,
				SuperUselessByte = 0x0A,
			};

			_data = Converter<TestClass>.Serialize(_testClass);

			_pm = new ProtocolMessage.ProtocolMessageManager();

			foreach (var benchmark in GetType()
								.GetMethods()
								.Where(x => x.GetCustomAttributes(true)
												.OfType<BenchmarkAttribute>()
												.Count() > 0))
			{
				benchmark.Invoke(this, null);
			}
		}

		[Benchmark]
		public object[] DecoratorSerialize()
			=> Converter<TestClass>.Serialize(_testClass);

		[Benchmark]
		public TestClass DecoratorDeserialize()
		{
			if (Converter<TestClass>.Deserialize(_data, out var result))
			{
				return result;
			}

			throw new Exception();
		}

		[Benchmark]
		public TestClass ProtocolMessageDeserialize()
			=> _pm.Convert<TestClass>(_data);

		[Benchmark]
		public TestClass DeserialiserDeserialize()
			=> Deserialiser<TestClass>.Deserialise(_data);
	}
}