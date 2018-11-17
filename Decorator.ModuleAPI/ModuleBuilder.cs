using System;
using System.Reflection;

namespace Decorator.ModuleAPI
{
	public static class ModuleBuilder
	{
		public static BaseModule Build(BaseContainer modContainer, IModuleAttribute moduleBuilder)
		{
			if (modContainer == null) throw new ArgumentNullException(nameof(modContainer));
			if (moduleBuilder == null) throw new ArgumentNullException(nameof(moduleBuilder));

			try
			{
				return (BaseModule)InvokeBuildMethod(
					moduleBuilder.GetType(),
					moduleBuilder.ModifyAppliedType(modContainer.Member.MemberType),
					nameof(IModuleAttribute.Build),
					moduleBuilder,
					modContainer);
			}
			catch (TargetInvocationException tie)
			{
				throw tie.InnerException;
			}
		}

		private static object InvokeBuildMethod(Type onType, Type makeGeneric, string name, object instance, BaseContainer modContainer)
			=> onType
				.GetMethod(name,
					BindingFlags.Public | BindingFlags.Instance,
					Type.DefaultBinder,
					new Type[]
					{
						typeof(BaseContainer),
					},
					null)
				.MakeGenericMethod(makeGeneric)
				.Invoke(instance, new object[] { modContainer });
	}
}