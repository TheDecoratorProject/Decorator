using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator.Attributes {
	/// <summary>When a message is successfully deserialized, it will use these attributes to find the proper handler for it.</summary>
	public class DeserializedHandlerAttribute : Attribute {
	}
}