using FluentAssertions;

using System;
using System.Reflection;

using Xunit;

namespace Decorator.Tests.Compiler
{
	public class CompilerTests
	{
		public class UnitTestAttribute : Attribute, IDecorationFactory
		{
			public Type GetType(PropertyInfo property) => property.PropertyType;

			public Type GetType(FieldInfo field) => field.FieldType;

			public IDecoration Make<T>(PropertyInfo property) => new UnitTestDecoration(property, null);

			public IDecoration Make<T>(FieldInfo field) => new UnitTestDecoration(null, field);

			public class UnitTestDecoration : IDecoration
			{
				public PropertyInfo Property { get; }
				public FieldInfo Field { get; }

				public UnitTestDecoration(PropertyInfo property, FieldInfo field)
				{
					Property = property;
					Field = field;
				}

				public bool Deserialize(ref object[] array, object instance, ref int index) => throw new NotImplementedException();

				public void EstimateSize(object instance, ref int size) => throw new NotImplementedException();

				public void Serialize(ref object[] array, object instance, ref int index) => throw new NotImplementedException();
			}
		}

		public static IDecoration[] Compile<T>()
			=> Compiler<T>.Compile(new Discovery<T>(), new DecorationFactoryBuilder());

		public static Action CreateCompileAction<T>()
			=> () => Compile<T>();

		// TODO:
		// refactor \/ this \/ to something better
		/*
				(decorations[1] as UnitTestAttribute.UnitTestDecoration).Field
					.Should()
					.BeSameAs(typeof(SampleClass)
						.GetField(nameof(SampleClass.Field)));
		*/

		public class SpacingBetweenProperties
		{
			public class ScenarioA
			{
				public class SampleClass
				{
					[Position(0), UnitTest] public string Property { get; set; }

					[Position(2), UnitTest] public string PropertyB { get; set; }
				}

				[Fact]
				public void OnCompilationInsertsIgnored()
				{
					var decorations = Compile<SampleClass>();

					(decorations[1] as Ignored)
						.Size
						.Should()
						.Be(1);
				}
			}

			public class ScenarioB
			{
				public class SampleClass
				{
					[Position(0), UnitTest] public string Property { get; set; }

					[Position(4), UnitTest] public string PropertyB { get; set; }

					[Position(8), UnitTest] public string PropertyC { get; set; }
				}

				[Fact]
				public void OnCompilationInsertsIgnored()
				{
					var decorations = Compile<SampleClass>();

					(decorations[1] as Ignored)
						.Size
						.Should()
						.Be(4 - 1);

					(decorations[3] as Ignored)
						.Size
						.Should()
						.Be(8 - 5);
				}
			}
		}

		public class CompilesNormally
		{
			public class SampleClass
			{
				[Position(0), UnitTest] public string Property { get; set; }

				[Position(1), UnitTest] public int Field;
			}

			[Fact]
			public void Compiles()
			{
				var decorations = Compile<SampleClass>();

				(decorations[0] as UnitTestAttribute.UnitTestDecoration).Property
					.Should()
					.BeSameAs(typeof(SampleClass)
						.GetProperty(nameof(SampleClass.Property)));

				(decorations[1] as UnitTestAttribute.UnitTestDecoration).Field
					.Should()
					.BeSameAs(typeof(SampleClass)
						.GetField(nameof(SampleClass.Field)));
			}
		}

		public class CompilesIntoOrder
		{
			public class SampleClass
			{
				[Position(2), UnitTest] public int A;
				[Position(0), UnitTest] public int B;
				[Position(1), UnitTest] public int C;
			}

			[Fact]
			public void CompilesIntoOrder_Test()
			{
				var decorations = Compile<SampleClass>();

				(decorations[0] as UnitTestAttribute.UnitTestDecoration).Field
					.Should()
					.BeSameAs(typeof(SampleClass)
						.GetField(nameof(SampleClass.B)));

				(decorations[1] as UnitTestAttribute.UnitTestDecoration).Field
					.Should()
					.BeSameAs(typeof(SampleClass)
						.GetField(nameof(SampleClass.C)));

				(decorations[2] as UnitTestAttribute.UnitTestDecoration).Field
					.Should()
					.BeSameAs(typeof(SampleClass)
						.GetField(nameof(SampleClass.A)));
			}
		}

		public class ThrowsOnNoPositionAttribute
		{
			public class SampleClass
			{
				[UnitTest] public string Test { get; set; }
			}

			[Fact]
			public void Throw()
			{
				CreateCompileAction<SampleClass>()
					.Should()
					.ThrowExactly<NoPositionAttributeException>();
			}
		}

		public class ThrowsOnDuplicatePositions
		{
			public class SampleClass
			{
				[Position(0), UnitTest] public string TestA { get; set; }
				[Position(0), UnitTest] public string TestB { get; set; }
			}

			[Fact]
			public void Throw()
			{
				CreateCompileAction<SampleClass>()
					.Should()
					.ThrowExactly<DuplicatePositionAttributeException>();
			}
		}
	}
}