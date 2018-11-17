using Newtonsoft.Json;

using System;

namespace Decorator.Examples
{
	public static class Helpers
	{
		public static void PrintContents(this object[] objectArray)
		{
			Console.WriteLine("{");

			for (int i = 0; i < objectArray.Length; i++)
			{
				Console.WriteLine($"  [{i}]:");

				if (objectArray == null)
				{
					Console.WriteLine("    null");
					continue;
				}

				Console.WriteLine($"    {objectArray[i].GetType()}");

				Console.WriteLine($"    {JsonConvert.SerializeObject(objectArray[i])}");
			}

			Console.WriteLine("}");
		}
	}
}