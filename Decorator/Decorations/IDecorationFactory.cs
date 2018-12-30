using System;
using System.Reflection;

namespace Decorator
{
	public interface IDecorationFactory
	{
		Type GetType(PropertyInfo propertyInfo);

		Type GetType(FieldInfo fieldInfo);

		IDecoration Make<T>(PropertyInfo propertyInfo);

		IDecoration Make<T>(FieldInfo fieldInfo);
	}
}