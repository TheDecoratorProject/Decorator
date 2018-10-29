using System;
using System.Reflection;

namespace Decorator
{
	internal static class GetTypeOfMemberInfo
	{
		public static Type GetMemberType(this MemberInfo memberInfo)
		{
			if (memberInfo is PropertyInfo propertyInfo)
			{
				return propertyInfo.PropertyType;
			}
			else if (memberInfo is FieldInfo fieldInfo)
			{
				return fieldInfo.FieldType;
			}
			else
			{
				throw new ArgumentException($"The {nameof(MemberInfo)} isn't a {(nameof(PropertyInfo))} or {nameof(FieldInfo)}. It's a [{memberInfo.GetType()}]", nameof(MemberInfo));
			}
		}
	}
}