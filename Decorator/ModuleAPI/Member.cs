using System;
using System.Reflection;

namespace Decorator.ModuleAPI
{
	public struct Member
	{
		internal Member(PropertyInfo property)
		{
			GetMember = property ?? throw new ArgumentNullException(nameof(property));

			MemberType = property.PropertyType;
		}

		internal Member(FieldInfo field)
		{
			GetMember = field ?? throw new ArgumentNullException(nameof(field));

			MemberType = field.FieldType;
		}

		public MemberInfo GetMember { get; }

		public Type MemberType { get; }
	}
}