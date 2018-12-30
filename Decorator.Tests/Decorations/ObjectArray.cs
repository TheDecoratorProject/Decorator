using FluentAssertions;

using System.Linq;

namespace Decorator.Tests.Decorations
{
	public class ObjectArray
	{
		public ObjectArray(int index = 1, object[] data = null)
		{
			Index = index;
			Data = data ?? Enumerable.Repeat<object>(null, 4).ToArray();
		}

		public int Index;
		public object[] Data;

		public void IndexIsNull(int index)
			=> Data[index]
			.Should()
			.BeNull();

		public void AllNullExcept(int index)
			=> Data
			.Where((o, i) => i != index) // where everyone BUT the index
			.Select(x => x == null) // is null
			.Where(x => false) // is anyone not null?
			.Should() // there should be NO not-nulls (everyone except index is nul;)
			.BeEmpty();
	}
}