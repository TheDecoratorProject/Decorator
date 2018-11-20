using Decorator.Attributes;
using Decorator.ModuleAPI;
using Decorator.Modules;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;

namespace Decorator.Tests.IL
{
	public class CompilesIL
	{
		public class TestClass
		{
			[Position(0), Required]
			public string MyProperty { get; set; }
		}

		[Fact]
		public void SupportsIL()
		{
			var c = new Compiler.Compiler<TestClass>();
			c.SupportsIL((i) =>
			{
				return new Container(((PropertyInfo)i).PropertyType, new Member((PropertyInfo)i));
			})
				.Should()
				.BeTrue();
		}

		[Fact]
		public void Deserializes()
		{
			var c = new Compiler.Compiler<TestClass>();

			var deserializer = c.CompileDeserializeIL((i) =>
			{
				return new Container(((PropertyInfo)i).PropertyType, new Member((PropertyInfo)i));
			});

			int l = 0;
			deserializer(new object[] { "testing 1, 2, 3" }, ref l, out var result)
				.Should().BeTrue();

			result.MyProperty.Should().Be("testing 1, 2, 3");
		}
	}
}
