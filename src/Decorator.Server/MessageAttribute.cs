using System;

namespace Decorator.Server
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class MessageAttribute : Attribute
	{
		public MessageAttribute(string type) => Type = type;

		public string Type { get; set; }
	}
}