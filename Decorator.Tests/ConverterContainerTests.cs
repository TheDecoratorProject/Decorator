using System.Collections.Generic;
using System.Collections.ObjectModel;
using Decorator.ModuleAPI;
using FluentAssertions;
using Xunit;

namespace Decorator.Tests
{
	public class ConverterContainerTests
	{
		public class MockConverter<T> : IConverter<T>
			where T : IDecorable, new()
		{
			public MockConverter(int id) => Id = id;

			public int Id { get; }

			public ReadOnlyCollection<BaseDecoratorModule> Members => throw new System.NotImplementedException();
			public object[] Serialize(T item) => throw new System.NotImplementedException();
			public bool TryDeserialize(object[] array, out T result) => throw new System.NotImplementedException();
			public bool TryDeserialize(object[] array, ref int arrayIndex, out T result) => throw new System.NotImplementedException();
		}

		public class MockConverterInstanceCreator : IConverterInstanceCreator
		{
			private int _counter;

			public IConverter<T> Create<T>(IDecoratorModuleCompiler<T> compiler, IConverterContainer container) where T : IDecorable, new()
				=> new MockConverter<T>(_counter++);

			public IDecoratorModuleCompiler<T> CreateCompiler<T>() where T : IDecorable, new()
				=> new DecoratorModuleCompiler<T>();
		}

		public ConverterContainer CreateContainer()
			=> new ConverterContainer(new MockConverterInstanceCreator());

		public void EnsureIdIs<T>(IConverter<T> converter, int id)
			where T : IDecorable, new()
			=> ((MockConverter<T>)converter).Id
				.Should()
				.Be(id);

		[Fact]
		public void CreatesNewInstancesWhenTheyDontExist()
		{
			var container = CreateContainer();

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsArrayAttributeBase>(),
				0);

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsFlattenArrayAttributeBase>(),
				1);

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsFlattenAttributeBase>(),
				2);
		}

		[Fact]
		public void ReusesOldInstancesWhenRequestingThemAgain()
		{
			var container = CreateContainer();

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsArrayAttributeBase>(),
				0);

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsFlattenArrayAttributeBase>(),
				1);

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsArrayAttributeBase>(),
				0);

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsFlattenArrayAttributeBase>(),
				1);

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsFlattenAttributeBase>(),
				2);

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsArrayAttributeBase>(),
				0);

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsFlattenArrayAttributeBase>(),
				1);

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsFlattenAttributeBase>(),
				2);
		}
	}
}