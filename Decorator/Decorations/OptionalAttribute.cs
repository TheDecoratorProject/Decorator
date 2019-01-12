using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class OptionalAttribute : Attribute, IDecorationFactory
	{
		public Type GetType(PropertyInfo property) => property.PropertyType;

		public Type GetType(FieldInfo field) => field.FieldType;

		public IDecoration Make<T>(PropertyInfo property) => MakeDecoration<T>(new RequiredAttribute().Make<T>(property));

		public IDecoration Make<T>(FieldInfo field) => MakeDecoration<T>(new RequiredAttribute().Make<T>(field));

		private OptionalDecoration<T> MakeDecoration<T>(IDecoration decoration)
			=> new OptionalDecoration<T>((RequiredAttribute.RequiredDecoration<T>)decoration);

		public class OptionalDecoration<T> : IDecoration
		{
			private readonly RequiredAttribute.RequiredDecoration<T> _decoration;

			public OptionalDecoration(RequiredAttribute.RequiredDecoration<T> decoration) => _decoration = decoration;

			public void Serialize(ref object[] array, object instance, ref int index) => _decoration.Serialize(ref array, instance, ref index);

			public bool Deserialize(ref object[] array, object instance, ref int index)
			{
				_decoration.Deserialize(ref array, instance, ref index);
				return true;
			}

			public void EstimateSize(object instance, ref int size) => _decoration.EstimateSize(instance, ref size);
		}
	}
}