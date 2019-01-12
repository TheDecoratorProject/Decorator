using Decorator.Attributes;
using Decorator.Converter;
using Decorator.Modules;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Decorator.Tests
{
	public class EnsureSupportsIL
	{
		public class RequiredTest
		{
			[Position(0), Required]
			public string Property { get; set; }
		}

		[Fact]
		public void RequiredSupportsIL()
			=> (TestConverter<RequiredTest>._ilconverter is ILConverter<RequiredTest>).Should().BeTrue();

		public class OptionalTest
		{
			[Position(0), Optional]
			public string Property { get; set; }
		}

		[Fact]
		public void OptionalSupportsIL()
			=> (TestConverter<RequiredTest>._ilconverter is ILConverter<RequiredTest>).Should().BeTrue();
	}

	/*
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
	public class TestAttribute : Attribute
	{
		public TestAttribute()
		{
		}
	}

	public class AttributeHackTest<T> where T : TestAttribute
	{
		[T]
		public string Property { get; set; }
	}
	*/
}
