﻿using System;
using System.Reflection;

namespace Deserialiser
{
	internal abstract class DeserialiseInfo
	{
		public readonly bool recurse;

		public DeserialiseInfo(bool recurse = false) => this.recurse = recurse;

		public abstract void Deserialise(ref object to, object[] values, ref int i);
	}

	internal abstract class DeserialiseInfo<T> : DeserialiseInfo
	{
		public DeserialiseInfo(bool recurse = false) : base(recurse)
		{
		}
	}

	internal class ValueInfo<T> : DeserialiseInfo<T>
	{
		public readonly Action<object, object> set;

		public ValueInfo(PropertyInfo info, bool recurse = false) : base(recurse)
			=> set = SwissILKnife.MemberUtils.GetSetMethod(info);

		public override void Deserialise(ref object to, object[] values, ref int i)
		{
			object o;

			if (recurse) o = Deserialiser<T>.Deserialise(values, ref i);
			else if (values[i] is T) o = values[i++];
			else throw new InvalidTypeException($"Expected type {typeof(T)} but instead got {values[i].GetType()}");

			set(to, o);
		}
	}

	internal class ConstInfo<T> : DeserialiseInfo<T>
	{
		public readonly object value;

		public ConstInfo(object value, bool recurse = false) : base(recurse) => this.value = value;

		public override void Deserialise(ref object to, object[] values, ref int i)
		{
			object val;
			if (recurse) val = Deserialiser<T>.Deserialise(values, ref i);
			else val = values[i++];
			if (!value.Equals(val)) throw new IncorrectValueException($"Expected {value} but instead got {val}");
		}
	}
}