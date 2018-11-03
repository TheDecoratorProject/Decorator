using System;
using System.Linq;
using System.Reflection;

namespace Decorator
{
	internal static class DecoratorInfoCreator
	{
		public static DecoratorInfo CreateDecoratorInfo(this Type type, MemberInfo memberInfo, params object[] extraInfo)
		{
			var invokeArgs = new object[] { memberInfo }.Concat(extraInfo).ToArray();

			var constructor = type.GetConstructor(invokeArgs.Select(x => x.GetType()).ToArray());
			var inv = constructor.Invoke(invokeArgs);

			return (DecoratorInfo)inv;
		}
	}
}