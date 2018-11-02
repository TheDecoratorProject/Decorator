﻿using SwissILKnife;

using System;
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

			IEnumerable<MemberInfo> members;

			var discoverAttributes = typeof(T)
										.GetCustomAttributes(true)
										.OfType<DiscoverAttribute>();

			if (discoverAttributes.Count() > 0)
			{
				// there are discover attributes
				// we will ONLY discover what they have specified in the discover attributes

				members = discoverAttributes
							.SelectMany(x => GetMembers(typeof(T), x.BindingFlags))
							.Where(x => x.GetCustomAttributes(true)
											.OfType<PositionAttribute>()
											.Count() > 0);
			}
			else
			{
				// no DiscoverAttribute?
				// we will just search for all public and instance ones then

				members = GetMembers(typeof(T), BindingFlags.Public | BindingFlags.Instance)
							.Where(x => x.GetCustomAttributes(true)
											.OfType<PositionAttribute>()
											.Count() > 0);
			}

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

		private static IEnumerable<MemberInfo> GetMembers(Type type, BindingFlags bindingFlags)
		{
			var props = type
							.GetProperties(bindingFlags)
							.Cast<MemberInfo>();

			var fields = type
							.GetFields(bindingFlags)
							.Cast<MemberInfo>();

			var members = props.Concat(fields);

			if (type.BaseType != default)
			{
				members = members.Concat(GetMembers(type.BaseType, bindingFlags));
			}

			return members
					.Where(x => x.GetCustomAttributes(true)
									.OfType<PositionAttribute>()
									.Count() > 0);
		}
	}
}