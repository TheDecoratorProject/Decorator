using Decorator.Attributes;
using Decorator.Modules;

using System;

namespace Decorator.Examples.Examples
{
	// Just a really basic example.
	public class Basic : DecoratorExample
	{
		// We have our class, and it *must* inherit IDecorable for Decorator to want to convert it.
		public class UserInformation : JsonString
		{
			// When we serialize our object[], the position attribute detonates
			// where in the result object[] that property will be.

			// At the 0th index in our array will be a string with our username.
			[Position(0), Required]
			public string UserName { get; set; }

			// At the 1th index in our array will be an integer with our favorite number.
			[Position(1), Required]
			public int FavoriteNumber { get; set; }
		}

		public override void Run()
		{
			// create some info
			var userInfo = new UserInformation
			{
				UserName = "SirJosh3917",
				FavoriteNumber = 1337,
			};

			// print it to the screen
			Console.WriteLine($"My {nameof(UserInformation)}:");
			userInfo.Print();

			// we will serialize it into an object[] here
			var data = DConverter<UserInformation>.Serialize(userInfo);

			Console.WriteLine($"Serialized {nameof(UserInformation)}:");
			data.PrintContents();

			// deserialize the data
			// guarenteed to be true since we didn't fiddle with data
			var success = DConverter<UserInformation>.TryDeserialize(data, out var deserializedUserInfo);

			Console.WriteLine($"Able to deserialize {nameof(data)}: {success}");

			if (success)
			{
				Console.WriteLine($"Deserialized user information:");

				// print out our new information.
				deserializedUserInfo.Print();
			}
		}
	}
}