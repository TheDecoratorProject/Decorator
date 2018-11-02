using FluentAssertions;

using System.Reflection;

using Xunit;

namespace Decorator.Tests
{
	public class DiscoverAttributeTests
	{
		public class DiscoverAttributeTestsBase : IDecorable
		{
			[Position(0), Required]
			public int PublicInstance;

			[Position(0), Required]
			private readonly int PrivateInstance;

			[Position(0), Required]
			public static int PublicStatic;

			[Position(0), Required]
			private static readonly int PrivateStatic;

			public int PrivateInstanceAccessor => PrivateInstance;
			public static int PrivateStaticAccessor => PrivateStatic;
		}

		private const int SUCCESS = 5;

		private T GetInst<T>() where T : IDecorable
		{
			if (!Converter<T>.TryDeserialize(new object[] { SUCCESS }, out var result))
			{
				throw new TestException(nameof(GetInst) + ", " + typeof(T));
			}

			return result;
		}

		[Discover(BindingFlags.Public | BindingFlags.Instance)]
		public class DiscoversPublicAndInstanceClass : DiscoverAttributeTestsBase { }

		[Fact]
		public void DiscoversPublicAndInstance()
			=> GetInst<DiscoversPublicAndInstanceClass>()
				.PublicInstance.Should().Be(SUCCESS);

		public class DiscoversDefaultClass : DiscoverAttributeTestsBase { }

		[Fact]
		public void DiscoversDefault()
			=> GetInst<DiscoversDefaultClass>()
				.PublicInstance.Should().Be(SUCCESS);

		[Discover(BindingFlags.NonPublic | BindingFlags.Instance)]
		public class DiscoversPrivateAndInstanceClass : DiscoverAttributeTestsBase { }

		[Fact]
		public void DiscoversPrivateAndInstance()
			=> GetInst<DiscoversPrivateAndInstanceClass>()
				.PrivateInstanceAccessor.Should().Be(SUCCESS);

		[Discover(BindingFlags.Public | BindingFlags.Static)]
		public class DiscoversPublicAndStaticClass : DiscoverAttributeTestsBase { }

		[Fact]
		public void DiscoversPublicAndStatic()
		{
			GetInst<DiscoversPublicAndStaticClass>();
			DiscoversPublicAndStaticClass.PublicStatic
				.Should().Be(SUCCESS);
		}

		[Discover(BindingFlags.NonPublic | BindingFlags.Static)]
		public class DiscoversPrivateAndStaticClass : DiscoverAttributeTestsBase { }

		[Fact]
		public void DiscoversPrivateAndStatic()
		{
			GetInst<DiscoversPrivateAndStaticClass>();
			DiscoversPrivateAndStaticClass.PrivateStaticAccessor
				.Should().Be(SUCCESS);
		}
	}
}