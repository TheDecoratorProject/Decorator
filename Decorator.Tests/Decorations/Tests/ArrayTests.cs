using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;

namespace Decorator.Tests.Decorations.Tests
{
	public class ArrayTests
	{
		public class SampleClass
		{
			public int[] IntArray { get; set; }

			public static PropertyInfo GetIntArrayInfo() => typeof(SampleClass).GetProperty(nameof(IntArray));

			public int[] FieldIntArray;

			public static FieldInfo GetFieldIntArrayInfo() => typeof(SampleClass).GetField(nameof(FieldIntArray));

			public string String { get; set; }
			public string FieldString;

			public static PropertyInfo GetStringInfo() => typeof(SampleClass).GetProperty(nameof(String));
			public static FieldInfo GetFieldStringInfo() => typeof(SampleClass).GetField(nameof(FieldString));
		}

		public static IDecoration GetArrayPropertyDecoration()
			=> new ArrayAttribute().Make<int>(SampleClass.GetIntArrayInfo());

		public static IDecoration GetArrayFieldDecoration()
			=> new ArrayAttribute().Make<int>(SampleClass.GetFieldIntArrayInfo());

		public class Properties
		{
			[Fact]
			public void GetsType()
				=> new ArrayAttribute()
				.GetType(SampleClass.GetIntArrayInfo())
				.Should()
				.Be(typeof(int));

			[Fact]
			public void ThrowsOnNonArray()
				=> ((Action)(() => new ArrayAttribute()
				.GetType(SampleClass.GetStringInfo())))
				.Should()
				.ThrowExactly<NotAnArrayException>();
		}

		public class Fields
		{
			[Fact]
			public void GetsType()
				=> new ArrayAttribute()
				.GetType(SampleClass.GetFieldIntArrayInfo())
				.Should()
				.Be(typeof(int));

			[Fact]
			public void ThrowsOnNonArray()
				=> ((Action)(() => new ArrayAttribute()
				.GetType(SampleClass.GetFieldStringInfo())))
				.Should()
				.ThrowExactly<NotAnArrayException>();
		}
	}
}
