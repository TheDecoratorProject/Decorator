using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
	/// <summary>The message class that is used for all serialization/deserialization transfers</summary>
    public class Message
    {
		public Message(string type, params object[] args) {
			this.Type = type;
			this.Args = args;
		}

		public string Type { get; }
		public object[] Args { get; }
    }
}
