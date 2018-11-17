using Decorator.Attributes;
using Decorator.Exceptions;
using Decorator.ModuleAPI;

using SwissILKnife;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator.Compiler
{
	public class Compiler<T> : ICompiler<T>
		where T : new()
	{
		public BaseModule[] Compile(Func<MemberInfo, BaseContainer> getContainer)
		{
			// cache constructor
			InstanceOf<T>.Create();

			// get all props/fields with pos attrib

			var members = DiscoverMembers();

			var dict = new SortedDictionary<int, BaseModule>();

			SetDecoratorModules(dict, members, getContainer);

			// fill up empty spaces with Ignored
			var last = dict.Keys.LastOrDefault();

			for (var i = 0; i < last; i++)
			{
				if (!dict.ContainsKey(i))
				{
					dict[i] = new IgnoredLogic();
				}
			}

			// save it as an array
			return dict.Values.ToArray();
		}

		private static void SetDecoratorModules(SortedDictionary<int, BaseModule> dictionary, IEnumerable<MemberInfo> members, Func<MemberInfo, BaseContainer> getContainer)
		{
			// for every member, get the DecoratorModule and store it in dict
			foreach (var i in members)
			{
				var builder = GetPairingOf(i);

				var decoratorModule =
					ModuleBuilder.Build(getContainer(i), builder);

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

				dictionary[positionAttribute.Position] = decoratorModule;
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

		private static IModuleAttribute GetPairingOf(MemberInfo member)
		{
			var attributes = member.GetCustomAttributes()
									.OfType<IModuleAttribute>();

			var attributesCount = attributes.Count();

			if (attributesCount < 1)
			{
				throw ExceptionManager.GetBrokenAttributePairing<PositionAttribute>
					(member.DeclaringType, member.Name, $"//TODO: FIX ME");
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