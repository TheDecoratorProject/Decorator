using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.IO;
using Xunit;

namespace Decorator.Tests
{
	/*
	public class Test : IDecorable
	{
		[Position(0), Required]
		public string Heyo { get; set; }

		[Position(2), Required]
		public int Woyo { get; set; }
	}

	public class ArrayTest : Test
	{
		[Position(0), Required]
		public string Heyo { get; set; }

		[Position(1), Array]
		public int[] ArrayData { get; set; }

		[Position(2), Required]
		public int Woyo { get; set; }
	}

	public class Perm : IDecorable
	{
		[Position(0), Required] public string PermName { get; set; }
		[Position(1), Required] public string PermPackage { get; set; }
		[Position(2), Required] public bool State { get; set; }
	}

	public class Player : IDecorable
	{
		[Position(0), Required] public string Username { get; set; }
		[Position(1), Required] public int Id { get; set; }
		[Position(2), Required] public bool Staff { get; set; }
		[Position(3), FlattenArray] public Perm[] Perms { get; set; }
	}

	public class World : IDecorable
	{
		[Position(0), Required] public string WorldName { get; set; }
		[Position(1), Required] public int OwnerId { get; set; }
		[Position(3), FlattenArray] public Player[] Players { get; set; }
	}

	public class UnitTest1
    {
		[Fact]
		public void Test5()
		{
			var world = new World
			{
				WorldName = "jojo's country side organic beans",
				OwnerId = 2,
				Players = new Player[]
				{
					new Player
					{
						Username = "John Wick",
						Id = -9000,
						Staff = false,
						Perms = new Perm[0],
					},
					new Player
					{
						Username = "SirJosh3917",
						Id = 1,
						Staff = true,
						Perms = new Perm[]
						{
							new Perm
							{
								PermName = "Staff",
								PermPackage = "game.staff",
								State = true
							},
							new Perm
							{
								PermName = "Vanished",
								PermPackage = "game.vanish.state",
								State = false
							},
						}
					},
					new Player
					{
						Username = "jojo",
						Id = 2,
						Staff = false,
						Perms = new Perm[]
						{
							new Perm
							{
								PermName = "Edit",
								PermPackage = "world.perms.canedit",
								State = true
							},
							new Perm
							{
								PermName = "Owner",
								PermPackage = "world.owner",
								State = true
							},
						}
					},
					new Player
					{
						Username = "johnny mc'm'gal'lon",
						Id = 3,
						Staff = false,
						Perms = new Perm[0],
					},
				}
			};

			File.WriteAllText("world.json", JsonConvert.SerializeObject(world, Formatting.Indented));

			var data = Converter<World>.Serialize(world);

			File.WriteAllText("objArr.json", JsonConvert.SerializeObject(data, Formatting.Indented));

			var des = Converter<World>.TryDeserialize(data, out var worldDes);
		}

        [Fact]
        public void Test1()
        {
            var data = new object[] { "1234", null, 5678 };

            Assert.True(Converter<Test>.TryDeserialize(data, out var item));

            Assert.Equal("1234", item.Heyo);
            Assert.Equal(5678, item.Woyo);
        }

        [Fact]
        public void Test2()
        {
            var item = new Test
            {
                Heyo = "1234",
                Woyo = 5678
            };

            var data = Converter<Test>.Serialize(item);

            Assert.Equal("1234", data[0]);
            Assert.Null(data[1]);
            Assert.Equal(5678, data[2]);
		}

		[Fact]
		public void Test3()
		{
			var item = new ArrayTest
			{
				Heyo = "5678",
				ArrayData = new int[]
				{
					0,
					5,
					18,
					27,
					1337,
					192,
					15
				},
				Woyo = 1234
			};

			var data = Converter<ArrayTest>.Serialize(item);

			Assert.Equal(item.Heyo, data[0]);
			Assert.Equal(item.ArrayData.Length, data[1]);
			for (int i = 0; i < item.ArrayData.Length; i++)
			{
				Assert.Equal(item.ArrayData[i], data[i + 2]);
			}
			Assert.Equal(item.Woyo, data[item.ArrayData.Length + 2]);
		}

		[Fact]
		public void Test4()
		{
			var item = new ArrayTest
			{
				Heyo = "5678",
				ArrayData = new int[]
				{
					0,
					5,
					18,
					27,
					1337,
					192,
					15
				},
				Woyo = 1234
			};

			var data = Converter<ArrayTest>.Serialize(item);
			Converter<ArrayTest>.TryDeserialize(data, out var des)
				.Should()
				.BeTrue();

			des.Heyo
				.Should()
				.Be(item.Heyo);

			des.ArrayData
				.Should()
				.BeEquivalentTo(item.ArrayData);

			des.Woyo
				.Should()
				.Be(item.Woyo);
		}
	}*/
}
