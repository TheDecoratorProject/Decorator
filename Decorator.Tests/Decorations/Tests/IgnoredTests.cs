using FluentAssertions;

using Xunit;

namespace Decorator.Tests.TestIgnored
{
	public class IgnoredTests
	{
		[Fact]
		public void Deserialize()
		{
			var ignored = new Ignored();

			var data = new object[1];
			var instance = new object();
			int index = 0;

			ignored.Deserialize(ref data, instance, ref index)
				.Should()
				.BeTrue();

			index
				.Should()
				.Be(1);
		}

		[Fact]
		public void Serializes()
		{
			var ignored = new Ignored();

			var data = new object[1];
			var instance = new object();
			int index = 0;

			ignored.Serialize(ref data, instance, ref index);

			index
				.Should()
				.Be(1);
		}

		[Fact]
		public void EstimatesSize()
		{
			var ignored = new Ignored();

			var instance = new object();
			int size = 0;

			ignored.EstimateSize(instance, ref size);

			size
				.Should()
				.Be(1);
		}
	}
}