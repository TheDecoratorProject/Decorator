using Decorator.ModuleAPI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Decorator.ModuleAPI
{
	public static class MemberHelper
	{
		public static Member GetMemberFrom(this MemberInfo memberInfo)
		{
			if (memberInfo is PropertyInfo propertyInfo)
			{
				return new Member(propertyInfo);
			}
			else if (memberInfo is FieldInfo fieldInfo)
			{
				return new Member(fieldInfo);
			}
			else
			{
				throw new InvalidOperationException($"The given {nameof(memberInfo)} isn't a {nameof(PropertyInfo)} or a {nameof(FieldInfo)}");
			}
		}
	}
}
