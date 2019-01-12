using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Decorator.Tests.Decorations.Tests
{
	// i'm so lazy i've given up on unit testing this class properly
	// it's less a unit test and more an integration test
	// but meh
	// my will to develop is so low RIP
	public class FlattenArrayTests
	{
		public class DataClass
		{
			[Required, Position(0)] public int Some { get; set; }
			[Array, Position(1)] public string[] Tags { get; set; }
			[Required, Position(2)] public bool On { get; set; }
		}

		public class ClassUsingData
		{
			[FlattenArray, Position(0)] public DataClass[] Datas { get; set; }
			[Array, Position(1)] public int[] Modifiers { get; set; }
		}

		[Fact]
		public void Works()
		{
			var cud = new ClassUsingData
			{
				Datas = new[]
				{
					new DataClass
					{
						Some = 5,
						Tags = new []{"test", "a", "b", "c"},
						On = true
					},

					new DataClass
					{
						Some = 6,
						Tags = new []{"a", "b", "test"},
						On = false
					},
				},

				Modifiers = new[]
				{
					1,
					8,
					9,
					2,
					5
				}
			};

			var cudSer = new object[]
			{
				2,
					5,
					4, "test", "a", "b", "c",
					true,

					6,
					3, "a", "b", "test",
					false,

				5,
					1, 8, 9, 2, 5
			};

			var ser = DDecorator<ClassUsingData>
				.Serialize(cud);
			 
			ser.Should()
				.BeEquivalentTo(cudSer);

			DDecorator<ClassUsingData>
				.TryDeserialize(ser, out var result)
				.Should()
				.BeTrue();

			result
				.Should()
				.BeEquivalentTo(cud);
		}
	}
}
