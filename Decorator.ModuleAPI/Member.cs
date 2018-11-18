using System;
using System.Reflection;

namespace Decorator.ModuleAPI
{
	public struct Member : IEquatable<Member>
	{
		public Member(PropertyInfo property)
		{
			GetMember = property ?? throw new ArgumentNullException(nameof(property));

			MemberType = property.PropertyType;
		}

		public Member(FieldInfo field)
		{
			GetMember = field ?? throw new ArgumentNullException(nameof(field));

			MemberType = field.FieldType;
		}

		public MemberInfo GetMember { get; }

		public Type MemberType { get; }

		public bool Equals(Member other)
		{
			if(other.GetMember == GetMember &&
				other.MemberType == MemberType)
			{
				return true;
			}

			return false;
		}
	}
}