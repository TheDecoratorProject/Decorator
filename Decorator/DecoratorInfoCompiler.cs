using SwissILKnife;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator
{
	internal static class DecoratorInfoContainer<T>
	{
		static DecoratorInfoContainer() => Members = DecoratorInfoCompiler<T>.Compile();

		public static DecoratorInfo[] Members;
	}

	internal static class DecoratorInfoCompiler<T>
	{
		public static DecoratorInfo[] Compile()
		{
			if (typeof(T).GetConstructors()
							.Count(x => x.GetParameters().Length == 0) == 0)
			{
				throw ExceptionManager.GetNoDefaultConstructor();
			}

			// cache constructor
			InstanceOf<T>.Create();

			// get all props/fields with pos attrib

			var members = DiscoverMembers();

			var dict = new SortedDictionary<int, DecoratorInfo>();

			SetDecoratorInfos(dict, members);

			// fill up empty spaces with Ignored
			var last = dict.Keys.LastOrDefault();

			for (var i = 0; i < last; i++)
			{
				if (!dict.ContainsKey(i))
				{
					dict[i] = new Ignored();
				}
			}

			// save it as an array
			return dict.Values.ToArray();
		}

		private static void SetDecoratorInfos(SortedDictionary<int, DecoratorInfo> dictionary, IEnumerable<MemberInfo> members)
		{

			// for every member, get the DecoratorInfo and store it in dict
			foreach (var i in members)
			{
				var decoratorInfo = GetPairingOf(i)
									.GetDecoratorInfo(i);

				var positionAttribute = i.GetCustomAttributes()
											.OfType<PositionAttribute>()
											.First();

				if (positionAttribute.Position < 0)
				{
					throw ExceptionManager.GetIrrationalAttributeValue<PositionAttribute>
						(typeof(T), positionAttribute.Position, "The value of the position attribute can't be less than 0");
				}

				if (dictionary.ContainsKey(positionAttribute.Position))
				{
					throw ExceptionManager.GetIrrationalAttributeValue<PositionAttribute>
						(typeof(T), positionAttribute.Position, $"There is already a member that contains this value ({dictionary[positionAttribute.Position]})");
				}

				dictionary[positionAttribute.Position] = decoratorInfo;
			}
		}

		private static IEnumerable<MemberInfo> DiscoverMembers()
		{
			var discoverAttributes = typeof(T)
										.GetCustomAttributes(true)
										.OfType<DiscoverAttribute>();

			if (discoverAttributes.Count() > 0)
			{
				// there are discover attributes
				// we will ONLY discover what they have specified in the discover attributes

				return discoverAttributes
							.SelectMany(x => typeof(T).GetMembersRecursively(x.BindingFlags))
							.Where(x => x.GetCustomAttributes(true)
											.OfType<PositionAttribute>()
											.Count() > 0);
			}
			else
			{
				// no DiscoverAttribute?
				// we will just search for all public and instance ones then

				return typeof(T).GetMembersRecursively(BindingFlags.Public | BindingFlags.Instance)
							.Where(x => x.GetCustomAttributes(true)
											.OfType<PositionAttribute>()
											.Count() > 0);
			}
		}

		private static IDecoratorInfoAttribute GetPairingOf(MemberInfo member)
		{
			var attributes = member.GetCustomAttributes()
									.OfType<IDecoratorInfoAttribute>();

			var attributesCount = attributes.Count();

			if (attributesCount < 1)
			{
				throw ExceptionManager.GetBrokenAttributePairing<PositionAttribute>
					(member.DeclaringType, member.Name, $"As a suggestion, could you add a {nameof(RequiredAttribute)} to it?");
			}
			else if (attributesCount > 1)
			{
				throw ExceptionManager.GetIrrationalAttribute
					("There are more modifiers then necessary, try removing a few.");
			}

			return attributes.First();
		}
	}
}