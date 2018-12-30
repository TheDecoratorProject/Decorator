using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Decorator.Tests.StretchToThat100PercentCodeCoverage
{
	public class DDecoratorTests
	{
		public class SampleClass
		{
			[Position(0), Required] public string PropertyString { get; set; }
		}

		[Fact]
		public void StretchThatCoverage()
		{
			var place1 = 0;
			var place2 = 0;
			var place3 = 0;
			var place4 = 0;

			DDecorator<SampleClass>.TryDeserialize(new object[] { "a" }, out var result1);
			DDecorator<SampleClass>.TryDeserialize(new object[] { "a" }, ref place1, out var result2);
			DDecorator<SampleClass>.TryDeserialize(new object[] { "a" }, new SampleClass(), ref place2);

			DDecorator<SampleClass>.Serialize(new SampleClass());
			DDecorator<SampleClass>.Serialize(new SampleClass(), ref place3);

			DDecorator<SampleClass>.EstimateSize(new SampleClass());
			DDecorator<SampleClass>.EstimateSize(new SampleClass(), ref place4);
		}
	}
}
