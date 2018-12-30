using System.Reflection;

namespace Decorator.Tests
{
	public interface IMemberTest<TMemberType, TFactory>
			where TFactory : IDecorationFactory
			where TMemberType : MemberInfo
	{
		void TypeMatches<TType>(TMemberType memberInfo);

		IDecoration GetDecoration<T>(TMemberType memberInfo);
	}
}