using Decorator.Tests.Decorations;

using FluentAssertions;

using System;
using System.Reflection;

using Xunit;

namespace Decorator.Tests.TestFlatten
{
	public class FlattenAttributeTests
	{
		public class Serializing
		{
			[Position(0), Required] public string String { get; set; }
			[Position(1), Required] public int Field;
		}

		public class SampleClass
		{
			public Serializing Instance { get; set; }

			public static PropertyInfo GetInstanceInfo() => typeof(SampleClass).GetProperty(nameof(Instance));

			public Serializing AnotherInstance;

			public static FieldInfo GetAnotherInstanceInfo() => typeof(SampleClass).GetField(nameof(AnotherInstance));
		}

		[Fact]
		public void ReturnsPropertyType() => new FlattenAttribute().GetType(SampleClass.GetInstanceInfo())
			.Should()
			.Be(typeof(Serializing));

		[Fact]
		public void ReturnsFieldType() => new FlattenAttribute().GetType(SampleClass.GetAnotherInstanceInfo())
			.Should()
			.Be(typeof(Serializing));

		public static IMemberTest<PropertyInfo, FlattenAttribute> GetPropertyTests() => new DecorationFactoryPropertyTests<FlattenAttribute>();

		public static IMemberTest<FieldInfo, FlattenAttribute> GetFieldTests() => new DecorationFactoryFieldTests<FlattenAttribute>();

		[Theory]
		[InlineData(new object[] { new object[] { } })]
		[InlineData(new object[] { new object[] { "a", 1 } })]
		[InlineData(new object[] { new object[] { 1, "a" } })]
		[InlineData(new object[] { new object[] { null } })]
		public void ShouldntThrowOn(object[] data)
		{
			int index = 0;

			new FlattenAttribute().Make<Serializing>(SampleClass.GetInstanceInfo())
				.Deserialize(ref data, new SampleClass(), ref index);
		}

		public class BaseTest<TClass, TAppliedTo, TMemberInfo, TFactory>
			where TClass : new()
			where TMemberInfo : MemberInfo
			where TFactory : IDecorationFactory
		{
			private readonly Func<TClass, TAppliedTo> _getInstance;
			private readonly IMemberTest<TMemberInfo, TFactory> _tester;
			private readonly TMemberInfo _getInstanceInfo;
			private object[] _data;
			private readonly TAppliedTo _equivalent;
			private readonly TClass _withEquivalent;

			public BaseTest
			(
				Func<TClass, TAppliedTo> getInstance,
				IMemberTest<TMemberInfo, TFactory> tester,
				TMemberInfo getInstanceInfo,
				object[] data,
				TAppliedTo equivalent,
				TClass withEquivalent
			)
			{
				_getInstance = getInstance;
				_tester = tester;
				_getInstanceInfo = getInstanceInfo;
				_data = data;
				_equivalent = equivalent;
				_withEquivalent = withEquivalent;
			}

			public void Deserialize()
			{
				var decoration = _tester.GetDecoration<TAppliedTo>(_getInstanceInfo);

				var instance = new TClass();
				var index = 0;

				decoration.Deserialize(ref _data, instance, ref index)
					.Should()
					.BeTrue();

				index
					.Should()
					.Be(_data.Length);

				_getInstance(instance)
					.Should()
					.BeEquivalentTo(_equivalent);
			}

			public void Serialize()
			{
				var decoration = _tester.GetDecoration<TAppliedTo>(_getInstanceInfo);

				var data = new object[_data.Length];
				var index = 0;

				decoration.Serialize(ref data, _withEquivalent, ref index);

				data
					.Should()
					.BeEquivalentTo(_data);
			}

			public void EstimateSize()
			{
				var decoration = _tester.GetDecoration<TAppliedTo>(_getInstanceInfo);

				var instance = new TClass();
				var size = 0;

				decoration.EstimateSize(instance, ref size);

				size
					.Should()
					.Be(_data.Length);
			}
		}

		public static class SerializingData
		{
			public static Serializing GetSerializing() => new Serializing { String = "str", Field = 123 };

			public static object[] GetData() => new object[] { "str", 123 };
		}

		public class PropertyTests
		{
			private BaseTest<SampleClass, Serializing, PropertyInfo, FlattenAttribute> GetInstance()
				=> new BaseTest<SampleClass, Serializing, PropertyInfo, FlattenAttribute>
				(
					getInstance: (instance) => instance.Instance,
					tester: GetPropertyTests(),
					getInstanceInfo: SampleClass.GetInstanceInfo(),
					data: SerializingData.GetData(),
					equivalent: SerializingData.GetSerializing(),
					withEquivalent: new SampleClass
					{
						Instance = SerializingData.GetSerializing(),
						AnotherInstance = default
					}
				);

			[Fact]
			public void Deserialize()
				=> GetInstance().Deserialize();

			[Fact]
			public void Serialize()
				=> GetInstance().Serialize();

			[Fact]
			public void EstimateSize()
				=> GetInstance().EstimateSize();
		}

		public class FieldTests
		{
			private BaseTest<SampleClass, Serializing, FieldInfo, FlattenAttribute> GetInstance()
				=> new BaseTest<SampleClass, Serializing, FieldInfo, FlattenAttribute>
				(
					getInstance: (instance) => instance.AnotherInstance,
					tester: GetFieldTests(),
					getInstanceInfo: SampleClass.GetAnotherInstanceInfo(),
					data: SerializingData.GetData(),
					equivalent: SerializingData.GetSerializing(),
					withEquivalent: new SampleClass
					{
						Instance = default,
						AnotherInstance = SerializingData.GetSerializing()
					}
				);

			[Fact]
			public void Deserialize()
				=> GetInstance().Deserialize();

			[Fact]
			public void Serialize()
				=> GetInstance().Serialize();

			[Fact]
			public void EstimateSize()
				=> GetInstance().EstimateSize();
		}

		public class ShouldntDeserializeTests
		{
			[Fact]
			public void DoesntDeserialize()
			{
				var decoration = GetPropertyTests().GetDecoration<Serializing>(SampleClass.GetInstanceInfo());

				var data = new object[] { 123, "lol" };
				var index = 0;

				decoration.Deserialize(ref data, new SampleClass(), ref index)
					.Should()
					.BeFalse();
			}
		}
	}
}