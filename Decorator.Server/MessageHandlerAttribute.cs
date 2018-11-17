using System;

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