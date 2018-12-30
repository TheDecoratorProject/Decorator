using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator
{
	public class Discovery<T> : IDiscovery<T>
	{
		public const BindingFlags DiscoveryFlags =
			BindingFlags.Public |
			BindingFlags.NonPublic |
			BindingFlags.Instance;

		public IEnumerable<PropertyInfo> FindProperties()
			=> typeof(T)
			.GetProperties(DiscoveryFlags)
			.Where(property => property.HasIDecorationFactoryAttribute());

		public IEnumerable<FieldInfo> FindFields()
			=> typeof(T)
			.GetFields(DiscoveryFlags)
			.Where(field => field.HasIDecorationFactoryAttribute());
	}

	internal static class Extension
	{
		public static bool HasIDecorationFactoryAttribute<TMemberInfo>(this TMemberInfo memberInfo)
			where TMemberInfo : MemberInfo
			=> memberInfo.CustomAttributes
				.Any
				(
					attribute => attribute.AttributeType
						.GetInterfaces()
						.Contains(typeof(IDecorationFactory))
				);
	}
}