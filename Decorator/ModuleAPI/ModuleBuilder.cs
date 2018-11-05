using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Decorator.ModuleAPI
{
	public static class ModuleBuilder
	{
		public static BaseDecoratorModule Build(Type appliedOn, MemberInfo member, IDecoratorModuleBuilder moduleBuilder)
		{
			var modified = moduleBuilder.ModifyAppliedType(appliedOn);

			try
			{
				return (BaseDecoratorModule)moduleBuilder.GetType()
												.GetMethod(nameof(IDecoratorModuleBuilder.Build),
															BindingFlags.Public | BindingFlags.Instance,
															Type.DefaultBinder,
															new Type[] {
															typeof(Type),
															typeof(MemberInfo)
																},
															null)
												.MakeGenericMethod(modified)
												.Invoke(moduleBuilder, new object[] { modified, member });
			}
			catch (TargetInvocationException tie)
			{
				throw tie.InnerException;
			}
		}
	}
}
