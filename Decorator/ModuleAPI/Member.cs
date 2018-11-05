using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Decorator.ModuleAPI
{
	public struct Member
	{
		internal Member(PropertyInfo property)
		{
			GetMember = property ?? throw new ArgumentNullException(nameof(property));
			Property = property;

			Field = null;
		}

		internal Member(FieldInfo field)
		{
			GetMember = field ?? throw new ArgumentNullException(nameof(field));

			Field = field;

			Property = null;
		}

		internal Member(bool ignored)
		{
			GetMember = null;
			Field = null;
			Property = null;
		}

		public MemberInfo GetMember { get; }

		public Type GetMemberType()
		{
			// impossible to not have a property or field

			if (Property != null)
			{
				return Property.PropertyType;
			}
			else if (Field != null)
			{
				return Field.FieldType;
			}
			else
			{
				throw new AccessViolationException($"You cannot get the type of a member whose property and field values are null.");
			}
		}

		public PropertyInfo Property { get; }
		public FieldInfo Field { get; }
	}
}
