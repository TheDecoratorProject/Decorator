using SwissILKnife;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator
{
	internal static class DecoratorInfoCompiler<T>
	{
		public static DecoratorInfo[] Members { get; }

		static DecoratorInfoCompiler()
		{
			// cache constructor
			InstanceOf<T>.Create();

			// get all props/fields with pos attrib

			var members = GetMembers(BindingFlags.Default)
							.Concat(
								typeof(T)
									.GetCustomAttributes(true)
									.OfType<DiscoverAttribute>()
									.SelectMany(x => GetMembers(x.BindingFlags))
								);

			var dict = new SortedDictionary<int, DecoratorInfo>();

			// for every member, get the DecoratorInfo and store it in dict
			foreach (var i in members)
			{
				var decoratorInfo = i.GetCustomAttributes()
					.OfType<IDecoratorInfoAttribute>()
					.First()
					.GetDecoratorInfo(i);

				dict[i.GetCustomAttributes()
						.OfType<PositionAttribute>()
						.First()
						.Position] = decoratorInfo;
			}

			// fill up empty spaces with Ignored
			var last = dict.Keys.LastOrDefault();

			for (int i = 0; i < last; i++)
			{
				if (!dict.ContainsKey(i))
				{
					dict[i] = new Ignored();
				}
			}

			// save it as an array
			Members = dict.Values.ToArray();
		}

		private static IEnumerable<MemberInfo> GetMembers(BindingFlags bindingFlags)
			=> typeof(T)
				.GetProperties(bindingFlags)
				.Cast<MemberInfo>()
				.Concat(typeof(T).GetFields(bindingFlags))
				.Where(x => x.GetCustomAttributes(true)
								.OfType<PositionAttribute>()
								.Count() > 0);
	}
}