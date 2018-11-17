using System;
using System.Linq;
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

			var modContainer = new ModuleContainer(moduleBuilder.ModifyAppliedType(member.MemberType), member, container);

			try
			{
				return (BaseDecoratorModule)InvokeBuildMethod(
					moduleBuilder.GetType(),
					modContainer.ModifiedType,
					nameof(IDecoratorModuleBuilder.Build),
					moduleBuilder,
					modContainer);
			}
			catch (TargetInvocationException tie)
			{
				throw tie.InnerException;
			}
		}

		private static object InvokeBuildMethod(Type onType, Type makeGeneric, string name, object instance, ModuleContainer modContainer)
			=> onType
				.GetMethod(name,
					BindingFlags.Public | BindingFlags.Instance,
					Type.DefaultBinder,
					new Type[]
					{
						typeof(ModuleContainer),
					},
					null)
				.MakeGenericMethod(makeGeneric)
				.Invoke(instance, new object[] { modContainer });

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