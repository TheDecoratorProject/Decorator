using Decorator.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Decorator.Tests
{
	[Message("ee")]
	[Repeatable]
	public class Stuff {

		[Position(0)]
		public string Username { get; set; }

		[Position(1)]
		public uint Id { get; set; }

		[Position(2)]
		public uint Coins { get; set; }

		public override bool Equals(object obj) {
			if(obj is Stuff s) {
				return s.Username.Equals(this.Username) &&
						s.Id.Equals(this.Id) &&
						s.Coins.Equals(this.Coins);
			}
			return false;
		}
	}

	public class EEHE {

		private Stuff[] GetEnumerable() {

			var itms = new Stuff[] {
				new Stuff {
					Username = "john",
					Id = 30,
					Coins = 133
				},
				new Stuff {
					Username = "cena",
					Id = 60,
					Coins = 131
				},
				new Stuff {
					Username = "scarce",
					Id = 1,
					Coins = uint.MaxValue
				}
			};

			return itms;
		}

		[Fact]
		public void A() {
			var itms = GetEnumerable();

			var msg = Serializer.SerializeEnumerable(itms);

			Assert.Equal("ee", msg.Type);
			Assert.Equal("john", msg.Args[0]);
			Assert.Equal("cena", msg.Args[3]);
			Assert.Equal("scarce", msg.Args[6]);
		}

		[Fact]
		public void B() {
			var itms = GetEnumerable();

			var msg = Serializer.SerializeEnumerable(itms);

			var msgDes = Deserializer.DeserializeToIEnumerable<Stuff>(msg).ToArray();

			Assert.Equal(itms, msgDes);
		}
	}
}
