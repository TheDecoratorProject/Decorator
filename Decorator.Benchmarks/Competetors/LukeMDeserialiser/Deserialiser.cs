using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Deserialiser
{
	public class Deserialiser<T>
	{
		private static readonly DeserialiseInfo[] info;

		private static Func<T> constructor = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();

		static Deserialiser()
		{
			Type t = typeof(T);

			Debug.WriteLine($"Initialising deserialiser for {t.ToString()}");

			if (t.GetCustomAttribute<DeserialisableAttribute>() == null)
				throw new NotDeserialisableException($"Cannot deserialise to type {t}");

			SortedDictionary<double, DeserialiseInfo> dInfo = new SortedDictionary<double, DeserialiseInfo>();

			foreach (ConstAttribute a in t.GetCustomAttributes<ConstAttribute>())
			{
				if (dInfo.ContainsKey(a.position)) throw new InvalidDeserialisationInfoException($"Multiple items found with position {a.position}");

				dInfo[a.position] = (DeserialiseInfo)typeof(ConstInfo<>)
					.MakeGenericType(a.value.GetType())
					.GetConstructor(new Type[] { typeof(object), typeof(bool) })
					.Invoke(new object[] { a.value, a.recurse });
			}

			foreach (PropertyInfo p in t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				OrderAttribute a = p.GetCustomAttribute<OrderAttribute>();
				if (a == null) continue;
				if (dInfo.ContainsKey(a.position)) throw new InvalidDeserialisationInfoException($"Multiple items found with position {a.position}");
				
				dInfo[a.position] = (DeserialiseInfo)typeof(ValueInfo<>)
					.MakeGenericType(p.PropertyType)
					.GetConstructor(new Type[] { typeof(PropertyInfo), typeof(bool) })
					.Invoke(new object[] { p, a.recurse });
			}

			info = dInfo.Values.ToArray();
		}

		public static T Deserialise(object[] values)
		{
			int i = 0;
			return Deserialise(values, ref i);
		}

		internal static T Deserialise(object[] values, ref int i)
		{
			object result = constructor();

			foreach (DeserialiseInfo d in info) d.Deserialise(ref result, values, ref i);

			return (T)result;
		}
	}
}
