using System;
using System.Reflection;

namespace Decorator.ModuleAPI
{
	public static class ModuleBuilder
	{
		public static BaseDecoratorModule Build(MemberInfo memberInfo, IConverterContainer container, IDecoratorModuleBuilder moduleBuilder)
		{
			if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));
			if (container == null) throw new ArgumentNullException(nameof(container));
			if (moduleBuilder == null) throw new ArgumentNullException(nameof(moduleBuilder));

			var member = GetMemberFrom(memberInfo);

			var mod = new ModuleContainer(moduleBuilder.ModifyAppliedType(member.MemberType), member, container);

			try
			{
				return (BaseDecoratorModule)moduleBuilder.GetType()
							.GetMethod(nameof(IDecoratorModuleBuilder.Build),
								BindingFlags.Public | BindingFlags.Instance,
								Type.DefaultBinder,
								new Type[] {
									typeof(ModuleContainer),
								},
								null)
							.MakeGenericMethod(mod.ModifiedType)
							.Invoke(moduleBuilder, new object[] { mod });
			}
			catch (TargetInvocationException tie)
			{
				throw tie.InnerException;
			}
		}

		private static Member GetMemberFrom(MemberInfo memberInfo)
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