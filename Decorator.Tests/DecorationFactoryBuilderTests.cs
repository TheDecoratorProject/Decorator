using FluentAssertions;

using System;
using System.Reflection;

using Xunit;

namespace Decorator.Tests.TestDecorationFactoryBuilder
{
	public class DecorationFactoryBuilderTests
	{
		public class UnitTestingDecoration : IDecoration
		{
			public bool Deserialize(ref object[] array, object instance, ref int index) => throw new NotImplementedException();

			public void EstimateSize(object instance, ref int size) => throw new NotImplementedException();

			public void Serialize(ref object[] array, object instance, ref int index) => throw new NotImplementedException();
		}

		public class UnitTestingFactory : IDecorationFactory
		{
			private readonly Type _returnType;
			private readonly IDecoration _returnDecoration;

			public UnitTestingFactory(Type returnType, IDecoration returnDecoration)
			{
				_returnType = returnType;
				_returnDecoration = returnDecoration;
			}

			public Type GetType(PropertyInfo property) => _returnType;

			public Type GetType(FieldInfo field) => _returnType;

			public IDecoration Make<T>(PropertyInfo property) => Check<T>();

			public IDecoration Make<T>(FieldInfo field) => Check<T>();

			private IDecoration Check<T>()
			{
				typeof(T)
					.Should()
					.Be(_returnType);

				return _returnDecoration;
			}
		}

		public class SampleClass
		{
			public string PropertyString { get; set; }

			public static PropertyInfo GetPropertyStringInfo() => typeof(SampleClass).GetProperty(nameof(PropertyString));

			public string FieldString { get; set; }

			public static PropertyInfo GetFieldStringInfo() => typeof(SampleClass).GetProperty(nameof(FieldString));
		}

		[Fact]
		public void CallsMakeWithCorrectTypeProperty()
		{
			var builder = new DecorationFactoryBuilder();

			var decoration = new UnitTestingDecoration();
			var property = SampleClass.GetPropertyStringInfo();

			builder.Build(new UnitTestingFactory(typeof(string), decoration), property)
				.Should()
				.Be(decoration);
		}

		[Fact]
		public void CallsMakeWithCorrectTypeField()
		{
			var builder = new DecorationFactoryBuilder();

			var decoration = new UnitTestingDecoration();
			var field = SampleClass.GetFieldStringInfo();

			builder.Build(new UnitTestingFactory(typeof(string), decoration), field)
				.Should()
				.Be(decoration);
		}
	}
}