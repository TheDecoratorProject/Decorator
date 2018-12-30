using System.Reflection;

namespace Decorator
{
	public interface IDecorationFactoryBuilder
	{
		IDecoration Build(IDecorationFactory factory, PropertyInfo propertyInfo);

		IDecoration Build(IDecorationFactory factory, FieldInfo fieldInfo);
	}
}