using Decorator.Caching;
using Decorator.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator
{
	public static class Deserializer
	{
		[Obsolete("This class only exists for backwards compatability, please try to upgrade to using Deserializer<TItem>.TryDeserializeItem insead.", false)]
		public static bool TryDeserializeItem<TItem>(BaseMessage m, out TItem result)
		{
			return Deserializer<TItem>.TryDeserializeItem(m, out result);
		}

		[Obsolete("This class only exists for backwards compatability, please try to upgrade to using Deserializer<TItem>.TryDeserializeItems insead.", false)]
		public static bool TryDeserializeItems<TItem>(BaseMessage m, out TItem[] result)
		{
			return Deserializer<TItem>.TryDeserializeItems(m, out result);
		}

		private static ClassWrapper _tryDeserialize;
		private static ConcurrentHashcodeDictionary<int, ILFunc> _tryDesItem;
		private static ConcurrentHashcodeDictionary<int, ILFunc> _tryDesItems;

		static Deserializer()
		{
			_tryDeserialize = new ClassWrapper(typeof(Deserializer<>));
			_tryDesItem = new ConcurrentHashcodeDictionary<int, ILFunc>();
			_tryDesItems = new ConcurrentHashcodeDictionary<int, ILFunc>();
		}

		/// <summary>
		/// Invokes <seealso cref="TryDeserializeItem{TItem}(BaseMessage, out TItem)"/> by using <seealso cref="System.Type"/> <paramref name="t"/> as the generic argument.
		/// </summary>
		/// <see cref="TryDeserializeItem{TItem}(BaseMessage, out TItem)"/>
		public static bool TryDeserializeItem(Type t, BaseMessage m, out object result)
		{
			if (null == t) throw new ArgumentNullException(nameof(t));
			if (null == m) throw new ArgumentNullException(nameof(m));

			var args = new object[] { m, null };

			var cls = _tryDeserialize.GetClassFor(t);

			if(!_tryDesItem.TryGetValue(cls.GetHashCode(), out var method))
			{
				var mthds = cls.GetMethods();

				method = cls.GetMethod(nameof(Deserializer<int>.TryDeserializeItem))
							.ILWrapRefSupport();

				_tryDesItem.TryAdd(cls.GetHashCode(), method);
			}

			if (!(bool)(method(null, args))) return TryMethodHelpers.EndTryMethod(false, default, out result);

			return TryMethodHelpers.EndTryMethod(true, args[1], out result);
		}

		/// <summary>
		/// Invokes <seealso cref="TryDeserializeItems{TItem}(BaseMessage, out TItem[])"/>
		/// </summary>
		/// <param name="t"></param>
		/// <param name="m"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static bool TryDeserializeItems(Type t, BaseMessage m, out object[] result)
		{
			if (null == t) throw new ArgumentNullException(nameof(t));
			if (null == m) throw new ArgumentNullException(nameof(m));

			var args = new object[] { m, null };

			var cls = _tryDeserialize.GetClassFor(t);

			if (!_tryDesItems.TryGetValue(cls.GetHashCode(), out var method))
			{
				var mthds = cls.GetMethods();

				method = cls.GetMethod(nameof(Deserializer<int>.TryDeserializeItems))
							.ILWrapRefSupport();

				_tryDesItems.TryAdd(cls.GetHashCode(), method);
			}

			if (!((bool)method(null, args))) return TryMethodHelpers.EndTryMethod(false, default, out result);

			return TryMethodHelpers.EndTryMethod(true, (object[])args[1], out result);
		}
	}
}
