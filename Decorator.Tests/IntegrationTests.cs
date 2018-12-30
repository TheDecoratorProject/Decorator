using FluentAssertions;

using System.Collections.Generic;

using Xunit;

namespace Decorator.Tests.Integration
{
	public class IntegrationTests
	{
		// maybe a message for a chat service or something
		public class Message
		{
			[Position(0), Required] public string Username { get; set; }

			[Position(1), Optional] public int RoleId { get; set; }

			// 2-3 are deprecated (in this chat message application)

			[Position(4), Required] public string MessageId { get; set; }
		}

		[Theory]
		[MemberData(nameof(Provider))]
		public void Deserializes(object[] array, Message instance)
		{
			if (instance != null)
			{
				DDecorator<Message>.TryDeserialize(array, out var result)
					.Should()
					.BeTrue();

				result
					.Should()
					.BeEquivalentTo(instance);
			}
			else
			{
				DDecorator<Message>.TryDeserialize(array, out var result)
					.Should()
					.BeFalse();
			}
		}

		[Theory]
		[MemberData(nameof(Provider))]
		public void Serializes(object[] array, Message instance)
		{
			if (instance == null)
			{
				return;
			}

			var result = DDecorator<Message>.Serialize(instance);

			// hotfix for optional attribute
			if (array[1].GetType() != typeof(int))
			{
				array[1] = default(int);
			}

			result
				.Should()
				.BeEquivalentTo(array);
		}

		public static IEnumerable<object[]> Provider()
		{
			yield return new object[]
			{
				new object[] { "SirJosh3917", 1337, null, null, "Hello, World!" },
				new Message
				{
					Username = "SirJosh3917",
					RoleId = 1337,
					MessageId = "Hello, World!"
				}
			};

			yield return new object[]
			{
				new object[] { "JohnDoe8989", "7", null, null, "Hiya, Frank!" },
				new Message
				{
					Username = "JohnDoe8989",
					RoleId = default,
					MessageId = "Hiya, Frank!"
				}
			};

			yield return new object[]
			{
				new object[] { 3, 3, 3, 3, 3 },
				null
			};

			yield break;
		}
	}
}