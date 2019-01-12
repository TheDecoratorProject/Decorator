using System;
using System.Reflection;

namespace Decorator
{
	public class DecorationFactoryBuilder : IDecorationFactoryBuilder
	{
		public IDecoration Build(IDecorationFactory factory, PropertyInfo propertyInfo)
			=> InvokeMake(factory.GetType(propertyInfo), factory, propertyInfo);

		public IDecoration Build(IDecorationFactory factory, FieldInfo fieldInfo)
			=> InvokeMake(factory.GetType(fieldInfo), factory, fieldInfo);

		private static IDecoration InvokeMake<TMemberInfo>(Type type, IDecorationFactory factory, TMemberInfo package)
			where TMemberInfo : MemberInfo
			=> (IDecoration)typeof(IDecorationFactory)
			.GetMethod(nameof(IDecorationFactory.Make), new Type[] { typeof(TMemberInfo) })
			.MakeGenericMethod(type)
			.Invoke(factory, new object[] { package });
	}
}