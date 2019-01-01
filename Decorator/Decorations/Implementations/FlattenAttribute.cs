using SwissILKnife;

using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class FlattenAttribute : Attribute, IDecorationFactory
	{
		public Type GetType(PropertyInfo propertyInfo) => propertyInfo.PropertyType;

		public Type GetType(FieldInfo fieldInfo) => fieldInfo.FieldType;

		public IDecoration Make<T>(PropertyInfo propertyInfo) => MakeDecoration<T>(propertyInfo);

		public IDecoration Make<T>(FieldInfo fieldInfo) => MakeDecoration<T>(fieldInfo);

		private IDecoration MakeDecoration<T>(MemberInfo memberInfo)
			=> new FlattenDecoration<T>
			(
				MemberUtils.GenerateGetMethod(memberInfo),
				MemberUtils.GenerateSetMethod(memberInfo)
			);

		public class FlattenDecoration<T> : IDecoration
		{
			// yea, it's using a static class.
			// but you know what? i've given up.
			// the code becomes a mess and completely unmaintainable
			// if i have to allow a way to pass in values
			// so no.
			// i won't be refactoring this to make it non-static.
			// i won't.
			// you can go ahead and submit a PR if you treasure it that much
			// you can go ahead and submit an issue detailing a fix if you need it so badly
			// because you know what? i've given up.
			// and you will too.

			private readonly IDecorator<T> _decorator;
			private readonly GetMethod _getMethod;
			private readonly SetMethod _setMethod;

			public FlattenDecoration(GetMethod getMethod, SetMethod setMethod)
			{
				_decorator = DDecorator<T>.Instance;
				_getMethod = getMethod;
				_getMethod = getMethod;
				_setMethod = setMethod;
				_setMethod = setMethod;
			}

			public void Serialize(ref object[] array, object instance, ref int index)
			{
				var indexCopy = index;

				var result = _decorator.Serialize((T)_getMethod(instance), ref index);

				for (int i = 0; i < result.Length; i++)
				{
					array[indexCopy + i] = result[i];
				}
			}

			public bool Deserialize(ref object[] array, object instance, ref int index)
			{
				if (!_decorator.TryDeserialize(array, ref index, out var result))
				{
					return false;
				}

				_setMethod(instance, result);
				return true;
			}

			public void EstimateSize(object instance, ref int size) => _decorator.EstimateSize((T)_getMethod(instance), ref size);
		}
	}
}