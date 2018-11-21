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
	public class IL_Required_ValueType
	{
		public class TestClass
		{
			[Position(0), Required]
			public int MyProperty { get; set; }
		}

		[Fact]
		public void SupportsIL()
		{
			var c = new Compiler.Compiler<TestClass>();
			c.SupportsIL(c.Compile((i) =>
			{
				return new Container(((PropertyInfo)i).PropertyType, new Member((PropertyInfo)i));
			}))
				.Should()
				.BeTrue();
		}

		[Fact]
		public void Deserializes()
		{
			var c = new Compiler.Compiler<TestClass>();

			var deserializer = c.CompileILDeserialize(c.Compile((i) =>
			{
				return new Container(((PropertyInfo)i).PropertyType, new Member((PropertyInfo)i));
			}));

			int l = 0;
			deserializer(new object[] { 1337 }, ref l, out var result)
				.Should().BeTrue();

			result.MyProperty.Should().Be(1337);
		}

		[Fact]
		public void Serializes()
		{
			var c = new Compiler.Compiler<TestClass>();

			var serializer = c.CompileILSerialize(c.Compile((i) =>
			{
				return new Container(((PropertyInfo)i).PropertyType, new Member((PropertyInfo)i));
			}));

			var result = serializer(new TestClass
			{
				MyProperty = 420
			});

			result
				.Should()
				.NotBeNull();

			result.Length
				.Should()
				.Be(1);

			result[0]
				.Should()
				.Be(420);
		}
	}
}
