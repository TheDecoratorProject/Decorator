using System;
using System.Reflection;

namespace Decorator.ModuleAPI
{
	public static class ModuleBuilder
	{
		public static BaseDecoratorModule Build(MemberInfo memberInfo, IDecoratorModuleBuilder moduleBuilder)
		{
			Type appliedOn;
			Member member;

			if (memberInfo is PropertyInfo propertyInfo)
			{
				appliedOn = propertyInfo.PropertyType;
				member = new Member(propertyInfo);
			}
			else if (memberInfo is FieldInfo fieldInfo)
			{
				appliedOn = fieldInfo.FieldType;
				member = new Member(fieldInfo);
			}
			else
			{
				throw new InvalidDeclarationException($"A module can only be applied to fields and properties. Not sure how you got this to throw, but lol.");
			}

			var modified = moduleBuilder.ModifyAppliedType(appliedOn);

			try
			{
				return (BaseDecoratorModule)moduleBuilder.GetType()
							.GetMethod(nameof(IDecoratorModuleBuilder.Build),
								BindingFlags.Public | BindingFlags.Instance,
								Type.DefaultBinder,
								new Type[] {
									typeof(Type),
									typeof(Member)
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