using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator.Attributes {
	/// <summary>When placed on a class, it allows deserialization to an array of this class.</summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class RepeatableAttribute : Attribute {
	}
}
