using System;
using System.Linq;
using System.Reflection;

namespace Decorator.ModuleAPI
{
	public static class EnsureIDecorable<TBuildModule>
		where TBuildModule : IOnlyDecorablesDecorableModuleBuilder
	{
		private static readonly Func<object, object[], object> _setter;

		public static DecoratorModule<T> InvokeBuild<T>(TBuildModule self, Type modifiedType, MemberInfo memberInfo)
		{
			if (typeof(T).GetInterfaces().Count(x => x == typeof(IDecorable)) == 0)
			{
				throw new InvalidDeclarationException($"{typeof(T)} does not inherit from {typeof(IDecorable)}.");
			}

			return (DecoratorModule<T>)typeof(TBuildModule)
				.GetMethod(nameof(IOnlyDecorablesDecorableModuleBuilder.BuildDecorable),
					BindingFlags.Public | BindingFlags.Instance,
					Type.DefaultBinder,
					new Type[]
					{
						typeof(Type),
						typeof(MemberInfo)
					},
					null)
				.MakeGenericMethod(typeof(T))
				.Invoke(self, new object[] { modifiedType, memberInfo });
		}
	}
}