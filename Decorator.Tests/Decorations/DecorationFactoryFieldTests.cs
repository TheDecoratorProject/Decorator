using FluentAssertions;

using System.Reflection;

namespace Decorator.Tests.Decorations
{
	public class DecorationFactoryFieldTests<TFactory> : IMemberTest<FieldInfo, TFactory> where TFactory : IDecorationFactory, new()
	{
		public void TypeMatches<TType>(FieldInfo fieldInfo)
			=> new TFactory()
			.GetType(fieldInfo)
			.Should()
			.Be(typeof(TType));

		public IDecoration GetDecoration<T>(FieldInfo fieldInfo)
			=> new TFactory()
			.Make<T>(fieldInfo);
	}
}