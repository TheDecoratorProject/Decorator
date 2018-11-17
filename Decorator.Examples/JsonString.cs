using Newtonsoft.Json;

using System;

namespace Decorator.Examples
{
	public abstract class JsonString
	{
		public void Print() => Console.WriteLine(this);

		public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
	}
}