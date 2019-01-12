using Decorator.Attributes;
using Decorator.Modules;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator.Examples
{
	// For the readme of decorator

	public class Overview : DecoratorExample
	{
		public class User
		{
			[Position(0), Required]
			public string Username { get; set; }

			[Position(1), Required]
			public int Id { get; set; }

			[Position(2), Required]
			public int Money { get; set; }
		}

		public override void Run()
		{
			User myUser = new User
			{
				Username = "SirJosh3917",
				Id = 1337,
				Money = 1_000_000,
			};

			object[] serializedUser = DConverter<User>.Serialize(myUser);
			Console.WriteLine(JsonConvert.SerializeObject(serializedUser));
			// outputs:
			// ["SirJosh3917",1337,1000000]

			object[] userData = new object[] { "Jeremy", 63, 1_000 };

			if (DConverter<User>.TryDeserialize(userData, out User deserializedUser))
			{
				// this gets evaluated

				Console.WriteLine(JsonConvert.SerializeObject(deserializedUser));
				// outputs:
				// {"Username":"Jeremy","Id":63,"Money":1000}
			}
		}
	}
}
