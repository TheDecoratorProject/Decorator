using System;
using System.Linq;
using System.Reflection;

namespace Decorator.Examples
{
	class Program
	{
		static void Main(string[] args)
		{
			var examples = Assembly.GetExecutingAssembly()
									.GetTypes()
									.Where(x => x.BaseType == typeof(DecoratorExample));

			var input = "";

			if (args.Length > 0)
			{
				input = args[0];
			}

			Func<Type, bool> selector;

			while (true)
			{
				selector = x => x.Name.ToLower() == input;

				if (examples.Count(selector) > 0)
				{
					var example = (DecoratorExample)examples.Where(selector)
															.First()
															.GetConstructor(new Type[] { })
															.Invoke(new object[0]);

					example.Run();

					Console.WriteLine($"\r\nExample Finished!");
					Console.ReadKey();
					Console.Clear();

					input = "";
				}
				else
				{
					foreach (var i in examples)
					{
						Console.WriteLine(i.Name);
					}

					Console.WriteLine();
					input = Console.ReadLine();
					Console.Clear();
				}
			}
		}
	}
}
