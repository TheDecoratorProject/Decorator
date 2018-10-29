using System;
using System.Reflection;

namespace Decorator
{
	internal static class DecoratorInfoCreator
	{
		public static DecoratorInfo CreateDecoratorInfo(this Type type, MemberInfo memberInfo)
		{
			var constructor = type.GetConstructor(Types.MemberInfoTypes);
			var inv = constructor.Invoke(new object[] { memberInfo });

			return (DecoratorInfo)inv;
		}
			/*
			=> (DecoratorInfo)type.GetConstructor(Types.MemberInfoTypes)
									.Invoke(new object[] { memberInfo });*/
	}
}