using Decorator.Attributes;
using Decorator.Modules;

using System;

namespace Decorator.Examples.Examples
{
	// This will feature the "Optional" attribute
	public class Optional : DecoratorExample
	{
		public class Message : JsonString
		{
			// The username of the user who sent the message
			[Position(0), Required]
			public string Username { get; set; }

			// What the text they sent was
			[Position(1), Required]
			public string Text { get; set; }

			// If they're 'God' (default is false)
			[Position(2), Optional]
			public bool God { get; set; } = false;
		}

		public override void Run()
		{
			var messageData = new object[][] {
				// Optional fields *must* have some kind of data to them - it can be anything, as long as it exists
				// e.g. 'null', or an integer (in case of h4x0r)

				// they'll be their default values if it's unable to deserialize it, but it being unable to deserialize
				// won't affect anything.
				new object[] { "SirJosh3917", "Hello all! I am the owner of this server!", true },
				new object[] { "Peasant", "I am a lonely citizen :(", false },
				new object[] { "h4x0r", "aM hAcKuR tRyNa RuIn YeR sTuFf", 1337 },
			};

			foreach (var i in messageData)
			{
				if (!DConverter<Message>.TryDeserialize(i, out var message))
				{
					// TODO: throw ThisShouldntHappenException()
					Console.WriteLine("Uh-oh! The example failed D:");
				}

				Console.WriteLine("Message data:");
				i.PrintContents();

				Console.WriteLine("Deserialized data:");
				message.Print();
			}
		}
	}
}