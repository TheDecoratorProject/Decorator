using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorator
{
	public class Compiler<T> : ICompiler<T>
	{
		private static ICompiler<T> ReusableCompilerInstance = new Compiler<T>();

		public static IDecoration[] Compile(IDiscovery<T> discovery, IDecorationFactoryBuilder builder)
			=> ReusableCompilerInstance.Compile(discovery, builder);

		IDecoration[] ICompiler<T>.Compile(IDiscovery<T> discovery, IDecorationFactoryBuilder builder)
		{
			var properties = discovery.FindProperties();
			var fields = discovery.FindFields();

			// build enumerable of (IDecorable, MemberInfo)
			var members =
				properties
				.Select
				(
					property =>
					(
						builder.Build(property.GetFactory(), property),
						(MemberInfo)property
					)
				)
				.Union
				(
					fields
					.Select
					(
						field =>
						(
							builder.Build(field.GetFactory(), field),
							(MemberInfo)field
						)
					)
				);

			var decorations = new SortedDictionary<uint, IDecoration>();

			var biggestPosition = 0u;

			// set each one into the dict
			foreach (var (decoration, memberInfo) in members)
			{
				var position = memberInfo.GetPosition().Position;

				if (decorations.ContainsKey(position))
				{
					throw new DuplicatePositionAttributeException();
				}

				decorations[position] = decoration;

				if (position > biggestPosition)
				{
					biggestPosition = position;
				}
			}

			var currentStretch = 0;

			// place an Ignored in place of wherever there isn't a position attribute
			for (var i = 0u; i < biggestPosition; i++)
			{
				if (!decorations.ContainsKey(i))
				{
					// a patch of places without a position attribute
					currentStretch++;
				}
				else
				{
					if (currentStretch > 0)
					{
						decorations[i - 1] = new Ignored(currentStretch);

						currentStretch = 0;
					}
				}
			}

			// TODO: refactor
			if (currentStretch > 0)
			{
				decorations[biggestPosition - 1] = new Ignored(currentStretch);

				currentStretch = 0;
			}

			return decorations.Values.ToArray();
		}
	}

	internal static class CompilerHelpers
	{
		// TODO: refactor out to a generic function?
		public static PositionAttribute GetPosition<TMemberInfo>(this TMemberInfo memberInfo)
			where TMemberInfo : MemberInfo
		{
			var attributes = memberInfo.GetCustomAttributes(typeof(PositionAttribute), true);

			if (!attributes.Any())
			{
				throw new NoPositionAttributeException();
			}

			return (PositionAttribute)attributes.First();
		}

		public static IDecorationFactory GetFactory<TMemberInfo>(this TMemberInfo memberInfo)
			where TMemberInfo : MemberInfo
		{
			return memberInfo.GetCustomAttributes()
				.OfType<IDecorationFactory>()
				.First();
			/*
			var attribute = memberInfo.CustomAttributes
				.First(x => x.AttributeType.GetInterfaces().Contains(typeof(IDecorationFactory)));

			attribute.
			*/
		}
	}
}