using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator.Server
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class MessageHandlerAttribute : Attribute
	{
		public MessageHandlerAttribute()
		{
		}
	}
}
