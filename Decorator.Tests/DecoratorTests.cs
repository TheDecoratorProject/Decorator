using FluentAssertions;

using Xunit;

namespace Decorator.Tests.Decorator
{
	public class DecoratorTests
	{
		public class SampleClass
		{
		}

		public class UnitTestingDecoration : IDecoration
		{
			public UnitTestingDecoration(int increment, int valueShouldBe, bool performShould = true)
			{
				_performShould = performShould;
				Increment = increment;
				ValueShouldBe = valueShouldBe;
			}

			public int Value { get; private set; }

			private readonly bool _performShould;

			public int Increment { get; }
			public int ValueShouldBe { get; }

			public bool Deserialize(ref object[] array, object instance, ref int index)
			{
				Record(ref index);

				if (_performShould)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			public void EstimateSize(object instance, ref int size) => Record(ref size);

			public void Serialize(ref object[] array, object instance, ref int index) => Record(ref index);

			public void Record(ref int value)
			{
				Value = value;

				if (_performShould)
				{
					Value
						.Should()
						.Be(ValueShouldBe);
				}

				value += Increment;
			}
		}

		public static UnitTestingDecoration[] GetDecorations()
			=> new UnitTestingDecoration[]
			{
				new UnitTestingDecoration(1, 0),
				new UnitTestingDecoration(2, 1),
				new UnitTestingDecoration(3, 3),
				new UnitTestingDecoration(4, 6),
				new UnitTestingDecoration(5, 10),
				new UnitTestingDecoration(0, 15)
			};

		public class Deserialize
		{
			[Fact]
			public void DeserializeTest()
			{
				var decorations = GetDecorations();
				var decorator = new Decorator<SampleClass>(decorations);

				var data = new object[] { };

				decorator.TryDeserialize(data, out var result)
					.Should().BeTrue();

				var lastDecoration = decorations[decorations.Length - 1];

				lastDecoration.Value
					.Should()
					.Be(lastDecoration.ValueShouldBe);
			}

			[Fact]
			public void DeserializeFails()
			{
				var decorations = GetDecorations();
				var decorator = new Decorator<SampleClass>(new UnitTestingDecoration[]
				{
					new UnitTestingDecoration(1, 0),
					new UnitTestingDecoration(2, 1),
					new UnitTestingDecoration(11, 10, false),
				});

				var data = new object[] { };

				decorator.TryDeserialize(data, out var result)
					.Should().BeFalse();
			}
		}

		public class Serialize
		{
			[Fact]
			public void SerializeTest()
			{
				var decorations = GetDecorations();
				var decorator = new Decorator<SampleClass>(decorations);

				var instance = new SampleClass();

				var result = decorator.Serialize(instance);

				result.Length
					.Should()
					.Be(decorator.EstimateSize(instance));

				var lastDecoration = decorations[decorations.Length - 1];

				lastDecoration.Value
					.Should()
					.Be(lastDecoration.ValueShouldBe);
			}
		}

		public class EstimateSize
		{
			[Fact]
			public void EstimateSizeTest()
			{
				var decorations = GetDecorations();
				var decorator = new Decorator<SampleClass>(decorations);

				var instance = new SampleClass();

				decorator.EstimateSize(instance)
					.Should()
					.Be(15);

				var lastDecoration = decorations[decorations.Length - 1];

				lastDecoration.Value
					.Should()
					.Be(lastDecoration.ValueShouldBe);
			}
		}
	}
}