using Decorator.Attributes;

using FluentAssertions;

using System.Linq;
using System.Reflection;

namespace Decorator.Tests.ModuleTests
{
	public static class Helpers
	{
		public static object[] GenerateAndCorrupt<T>(int pos)
			where T : new()
		{
			var item = new T();
			var result = DConverter<T>.Serialize(item);

			if (result[pos].GetType() == typeof(int))
			{
				result[pos] = "__corrupt__";
			}
			else
			{
				result[pos] = 1030307;
			}

			return result;
		}

		public static int EndsOn<T>(object[] deserialize)
			where T : new()
		{
			var position = 0;

			DConverter<T>.TryDeserialize(deserialize, ref position, out _)
				.Should().Be(false, "Ensure the data being modified is corrupt.\r\nIf this happens to sometimes pass, please revise the data corruptor.");

			return position;
		}

		public static int LengthOfDefault<T>()
			where T : new()
			=> DConverter<T>.Serialize(new T()).Length;

		public static PropertyInfo[] GetProperties<T>()
			where T : new()
			=> typeof(T)
				.GetProperties()
				.Where(x => x.GetCustomAttributes(true).OfType<PositionAttribute>().Count() > 0)
				.OrderBy(x => x.GetCustomAttributes(true).OfType<PositionAttribute>().First().Position)
				.ToArray();
	}
}