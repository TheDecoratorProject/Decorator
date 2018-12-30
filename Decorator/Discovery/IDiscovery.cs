using System.Collections.Generic;
using System.Reflection;

namespace Decorator
{
	public interface IDiscovery<T>
	{
		IEnumerable<PropertyInfo> FindProperties();

		IEnumerable<FieldInfo> FindFields();
	}
}