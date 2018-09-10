using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
    internal static class ListHelper
    {
		public static void MagicInsert<T>(this List<T> list, int pos, T item) {
			while (list.Count <= pos)
				list.Add(default);

			list[pos] = item;
		}
    }
}
