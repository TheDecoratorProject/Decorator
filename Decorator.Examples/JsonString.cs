using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator.Examples
{
	public abstract class JsonString
	{
		public void Print() => Console.WriteLine(this);
		public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
	}
}
