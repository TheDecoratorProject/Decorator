using BenchmarkDotNet.Attributes;

using Deserialiser;

using System;
using System.Linq;

namespace Decorator.Benchmarks
{
	public class ShouldNotHappenException : Exception
	{
		public ShouldNotHappenException() : base("This shouldn't happen.")
		{
		}
	}

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
		private readonly TestClass _testClass;
		private readonly object[] _data;
		private readonly ProtocolMessage.ProtocolMessageManager _pm;

		public Benchmarks()
		{
			_testClass = new TestClass
			{
				String = "Hello, World!",
				Int = 1234,
				SomeIntegers = new[]
				{
					1, 2, 8, 9, 20, 15
				},
				LongInt = 1106300L,
				UltraLongInt = 11063002uL,
				Identifier = 0xFF,
				SuperUselessByte = 0x0A,
			};

			_data = DConverter<TestClass>.Serialize(_testClass);

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
			=> DConverter<TestClass>.Serialize(_testClass);

		[Benchmark]
		public TestClass DecoratorDeserialize()
		{
			if (DConverter<TestClass>.TryDeserialize(_data, out var result))
			{
				return result;
			}

			throw new ShouldNotHappenException();
		}

		[Benchmark]
		public TestClass ECDecoratorDeserialize()
		{
			var i = 0;
			if (DConverter<TestClass>.TryDeserialize(_data, ref i, out var result))
			{
				return result;
			}

			throw new ShouldNotHappenException();
		}

		[Benchmark]
		public TestClass ProtocolMessageDeserialize()
			=> _pm.Convert<TestClass>(_data);

		[Benchmark]
		public TestClass DeserialiserDeserialize()
			=> Deserialiser<TestClass>.Deserialise(_data);
	}
}