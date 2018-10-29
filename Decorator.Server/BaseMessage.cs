using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator.Server
{
	public struct BaseMessage
	{
		public BaseMessage(string type, params object[] arguments)
		{
			Type = type;
			Arguments = arguments;
		}

		public string Type { get; }
		public object[] Arguments { get; }
	}
}
