using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;

namespace Decorator.Tests.Decorations.Tests
{
	// i am SOOO lazy i do NOT want to do this
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

		public class BaseTest
		{
			private readonly Func<SampleClass, int[]> _getArray;
			private readonly IDecoration _decoration;

			public BaseTest(Func<SampleClass, int[]> getArray, IDecoration decoration)
			{
				_getArray = getArray;
				_decoration = decoration;
			}

			public void Deserialize(object[] data, int[] shouldBe)
			{
				var i = 0;
				var instance = new SampleClass();
				_decoration.Deserialize(ref data, instance, ref i)
					.Should().BeTrue();

				_getArray(instance)
					.Should()
					.BeEquivalentTo(shouldBe);
			}

			public void Serialize(SampleClass instance, object[] shouldBe)
			{
				var i = 0;
				var newArr = new object[shouldBe.Length];
				_decoration.Serialize(ref newArr, instance, ref i);
			}

			public void EstimateSize(SampleClass instance, int sizeShouldBe)
			{
				var size = 0;
				_decoration.EstimateSize(instance, ref size);

				size.Should()
					.Be(sizeShouldBe);
			}
		}

		public static class Get
		{
			public static (object[], int[], int) Data()
				=>
				(
					new object[] { 6, 0, 1, 2, 3, 4, 5 },
					new int[] { 0, 1, 2, 3, 4, 5 },
					7
				);
		}

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

			// TODO: less repetition
			// i'm heckkin' lazy

			[Fact]
			public void Deserializes()
				=> new BaseTest((inst) => inst.IntArray, new ArrayAttribute().Make<int>(SampleClass.GetIntArrayInfo()))
				.Deserialize(Get.Data().Item1, Get.Data().Item2);

			[Fact]
			public void Serializes()
				=> new BaseTest((inst) => inst.IntArray, new ArrayAttribute().Make<int>(SampleClass.GetIntArrayInfo()))
				.Serialize(new SampleClass { IntArray = Get.Data().Item2 }, Get.Data().Item1);

			[Fact]
			public void EstimatesSize()
				=> new BaseTest((inst) => inst.IntArray, new ArrayAttribute().Make<int>(SampleClass.GetIntArrayInfo()))
				.EstimateSize(new SampleClass { IntArray = Get.Data().Item2 }, Get.Data().Item3);
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

			// TODO: less repetition
			// i'm heckkin' lazy

			[Fact]
			public void Deserializes()
				=> new BaseTest((inst) => inst.FieldIntArray, new ArrayAttribute().Make<int>(SampleClass.GetFieldIntArrayInfo()))
				.Deserialize(Get.Data().Item1, Get.Data().Item2);

			[Fact]
			public void Serializes()
				=> new BaseTest((inst) => inst.FieldIntArray, new ArrayAttribute().Make<int>(SampleClass.GetFieldIntArrayInfo()))
				.Serialize(new SampleClass { FieldIntArray = Get.Data().Item2 }, Get.Data().Item1);

			[Fact]
			public void EstimatesSize()
				=> new BaseTest((inst) => inst.FieldIntArray, new ArrayAttribute().Make<int>(SampleClass.GetFieldIntArrayInfo()))
				.EstimateSize(new SampleClass { FieldIntArray = Get.Data().Item2 }, Get.Data().Item3);
		}
	}
}
