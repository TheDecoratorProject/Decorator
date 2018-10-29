using System.Reflection;

namespace Decorator
{
	internal interface IDecoratorInfoAttribute
	{
		DecoratorInfo GetDecoratorInfo(MemberInfo memberValue);
	}
}