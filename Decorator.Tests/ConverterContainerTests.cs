using Decorator.ModuleAPI;

using FluentAssertions;

using System.Collections.ObjectModel;

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

			public ReadOnlyCollection<BaseDecoratorModule> Members => new ReadOnlyCollection<BaseDecoratorModule>(new BaseDecoratorModule[] { });

			public object[] Serialize(T item) => throw new System.NotImplementedException();

			public bool TryDeserialize(object[] array, out T result) => throw new System.NotImplementedException();

			public bool TryDeserialize(object[] array, ref int arrayIndex, out T result) => throw new System.NotImplementedException();
		}

		public class MockConverterInstanceCreator : IConverterInstanceCreator
		{
			private int _counter;

			public IConverter<T> Create<T>(BaseDecoratorModule[] members) where T : IDecorable, new()
				=> new MockConverter<T>(_counter++);

			public IDecoratorModuleCompiler<T> CreateCompiler<T>() where T : IDecorable, new()
				=> new DecoratorModuleCompiler<T>();
		}

		public static ConverterContainer CreateContainer()
			=> new ConverterContainer(new MockConverterInstanceCreator());

		public static void EnsureIdIs<T>(IConverter<T> converter, int id)
			where T : IDecorable, new()
			=> ((MockConverter<T>)converter).Id
				.Should()
				.Be(id);

		[Fact]
		public void CreatesNewInstancesWhenTheyDontExist()
		{
			var container = CreateContainer();

			var req0 = (MockConverter<DeserializationTests.DeserializationTestsArrayAttributeBase>)container.Request<DeserializationTests.DeserializationTestsArrayAttributeBase>();
			var req1 = (MockConverter<DeserializationTests.DeserializationTestsFlattenArrayAttributeBase>)container.Request<DeserializationTests.DeserializationTestsFlattenArrayAttributeBase>();
			var req2 = (MockConverter<DeserializationTests.DeserializationTestsFlattenAttributeBase>)container.Request<DeserializationTests.DeserializationTestsFlattenAttributeBase>();

			EnsureIdIs(
				req0,
				req0.Id);

			EnsureIdIs(
				req1,
				req1.Id);

			EnsureIdIs(
				req2,
				req2.Id);
		}

		[Fact]
		public void ReusesOldInstancesWhenRequestingThemAgain()
		{
			var container = CreateContainer();

			var req0 = (MockConverter<DeserializationTests.DeserializationTestsArrayAttributeBase>)container.Request<DeserializationTests.DeserializationTestsArrayAttributeBase>();

			EnsureIdIs(
				req0,
				req0.Id);

			var req1 = (MockConverter<DeserializationTests.DeserializationTestsFlattenArrayAttributeBase>)container.Request<DeserializationTests.DeserializationTestsFlattenArrayAttributeBase>();

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsArrayAttributeBase>(),
			req0.Id);

			EnsureIdIs(
				req1,
				req1.Id);

			var req2 = (MockConverter<DeserializationTests.DeserializationTestsFlattenAttributeBase>)container.Request<DeserializationTests.DeserializationTestsFlattenAttributeBase>();

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsArrayAttributeBase>(),
			req0.Id);

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsFlattenArrayAttributeBase>(),
			req1.Id);

			EnsureIdIs(
				req2,
				req2.Id);



			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsArrayAttributeBase>(),
				req0.Id);

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsFlattenArrayAttributeBase>(),
				req1.Id);

			EnsureIdIs(
				container.Request<DeserializationTests.DeserializationTestsFlattenAttributeBase>(),
				req2.Id);
		}
	}
}