using System.Reflection;

namespace Decorator.Tests.Decorations
{
	public static class DeserializationTesting
	{
		public static bool Deserialize<T>(IDecorationFactory factory, PropertyInfo propertyInfo, ref object[] array, object instance, ref int index)
			=> factory.Make<T>(propertyInfo).Deserialize(ref array, instance, ref index);

		public static bool Deserialize<T>(IDecorationFactory factory, FieldInfo fieldInfo, ref object[] array, object instance, ref int index)
			=> factory.Make<T>(fieldInfo).Deserialize(ref array, instance, ref index);
	}
}