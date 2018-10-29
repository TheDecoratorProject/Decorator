using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class DiscoverAttribute : Attribute
	{
		public DiscoverAttribute(BindingFlags bindingFlags)
		{
			BindingFlags = bindingFlags;
		}

		public BindingFlags BindingFlags { get; set; }
	}
}
