using FluentAssertions;

using System;
using System.Reflection;

using Xunit;

namespace Decorator.Tests.Discovery
{
	public class DiscoveryTests
	{
		public class UnitTestDecorationFactoryAttribute : Attribute, IDecorationFactory
		{
			public Type GetType(PropertyInfo property) => throw new NotImplementedException();

			public Type GetType(FieldInfo field) => throw new NotImplementedException();

			public IDecoration Make<T>(PropertyInfo property) => throw new NotImplementedException();

			public IDecoration Make<T>(FieldInfo field) => throw new NotImplementedException();
		}

		public class SampleClass
		{
			public const BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Instance;

			[UnitTestDecorationFactory] public string PropertyString { get; set; }

			public static PropertyInfo GetPropertyStringInfo() => typeof(SampleClass).GetProperty(nameof(PropertyString));

			[UnitTestDecorationFactory] public int PropertyInt { get; set; }

			public static PropertyInfo GetPropertyIntInfo() => typeof(SampleClass).GetProperty(nameof(PropertyInt));

			[UnitTestDecorationFactory] public string FieldString;

			public static FieldInfo GetFieldStringInfo() => typeof(SampleClass).GetField(nameof(FieldString));

			[UnitTestDecorationFactory] public int FieldInt;

			public static FieldInfo GetFieldIntInfo() => typeof(SampleClass).GetField(nameof(FieldInt));

			[UnitTestDecorationFactory] private string PrivatePropertyString { get; set; }

			public static PropertyInfo GetPrivatePropertyStringInfo() => typeof(SampleClass).GetProperty(nameof(PrivatePropertyString), Private);

			[UnitTestDecorationFactory] private int PrivatePropertyInt { get; set; }

			public static PropertyInfo GetPrivatePropertyIntInfo() => typeof(SampleClass).GetProperty(nameof(PrivatePropertyInt), Private);

			[UnitTestDecorationFactory] private readonly string PrivateFieldString;

			public static FieldInfo GetPrivateFieldStringInfo() => typeof(SampleClass).GetField(nameof(PrivateFieldString), Private);

			[UnitTestDecorationFactory] private readonly int PrivateFieldInt;

			public static FieldInfo GetPrivateFieldIntInfo() => typeof(SampleClass).GetField(nameof(PrivateFieldInt), Private);

			public string DontDiscoverPropertyString { get; set; }

			public int DontDiscoverPropertyInt { get; set; }

			public string DontDiscoverFieldString;

			public int DontDiscoverFieldInt;

			private string DontDiscoverPrivatePropertyString { get; set; }

			private int DontDiscoverPrivatePropertyInt { get; set; }

			private readonly string DontDiscoverPrivateFieldString;

			private readonly int DontDiscoverPrivateFieldInt;
		}

		public class DiscoverProperties
		{
			[Fact]
			public void FindsProperties()
			{
				var shouldFind = new PropertyInfo[]
				{
					SampleClass.GetPropertyStringInfo(),
					SampleClass.GetPropertyIntInfo(),

					SampleClass.GetPrivatePropertyStringInfo(),
					SampleClass.GetPrivatePropertyIntInfo(),
				};

				var finder = new Discovery<SampleClass>();

				finder.FindProperties()
					.Should()
					.BeEquivalentTo(shouldFind);
			}
		}

		public class DiscoverFields
		{
			[Fact]
			public void FindsFields()
			{
				var shouldFind = new FieldInfo[]
				{
					SampleClass.GetFieldStringInfo(),
					SampleClass.GetFieldIntInfo(),

					SampleClass.GetPrivateFieldStringInfo(),
					SampleClass.GetPrivateFieldIntInfo(),
				};

				var finder = new Discovery<SampleClass>();

				finder.FindFields()
					.Should()
					.BeEquivalentTo(shouldFind);
			}
		}
	}
}