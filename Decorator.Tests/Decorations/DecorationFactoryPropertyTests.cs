using FluentAssertions;

using System.Reflection;

namespace Decorator.Tests.Decorations
{
	public class DecorationFactoryPropertyTests<TFactory> : IMemberTest<PropertyInfo, TFactory> where TFactory : IDecorationFactory, new()
	{
		public void TypeMatches<TType>(PropertyInfo propertyInfo)
			=> new TFactory()
			.GetType(propertyInfo)
			.Should()
			.Be(typeof(TType));

		public IDecoration GetDecoration<T>(PropertyInfo propertyInfo)
			=> new TFactory()
			.Make<T>(propertyInfo);
	}
}