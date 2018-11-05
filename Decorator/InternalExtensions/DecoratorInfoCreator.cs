using Decorator.ModuleAPI;
using System;
using System.Linq;
using System.Reflection;

namespace Decorator
{
	internal static class DecoratorInfoCreator
	{
		public static BaseDecoratorModule CreateDecoratorInfo(this Type type, Type modifiedType, MemberInfo memberInfo, params object[] extraInfo)
		{
			try
			{
				var invokeArgs = new object[] { modifiedType, memberInfo }.Concat(extraInfo).ToArray();

				var constructor = type.GetConstructor(invokeArgs.Select(x => x.GetType()).ToArray());
				var inv = constructor.Invoke(invokeArgs);

				return (BaseDecoratorModule)inv;
			}
			catch (TargetInvocationException tie)
			{
				throw tie.InnerException;
			}
		}
	}
}