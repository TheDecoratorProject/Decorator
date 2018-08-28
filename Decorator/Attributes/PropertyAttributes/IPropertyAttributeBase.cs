using System.Reflection;

namespace Decorator.Attributes {

	internal interface IPropertyAttributeBase {

		//TODO: return custom class with int & bool
		bool CheckRequirements<T>(PropertyInfo propInfo, Message msg, T item, PositionAttribute pos);
	}
}