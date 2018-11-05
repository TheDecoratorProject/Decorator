﻿using Decorator.ModuleAPI;

using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ArrayAttribute : Attribute, IDecoratorModuleBuilder
	{
		public ArrayAttribute() : this(0xFFFF)
		{
		}

		public ArrayAttribute(int maxArraySize)
			=> MaxArraySize = maxArraySize;

		public int MaxArraySize { get; set; }

		public Type ModifyAppliedType(Type attributeAppliedTo)
		{
			if (!attributeAppliedTo.IsArray)
			{
				throw new InvalidDeclarationException($"{attributeAppliedTo} must be an array!");
			}

			return attributeAppliedTo.GetElementType();
		}

		public DecoratorModule<T> Build<T>(Type modifiedType, MemberInfo memberInfo)
			=> new Module<T>(modifiedType, memberInfo, MaxArraySize);

		public class Module<T> : DecoratorModule<T>
		{
			public Module(Type modifiedType, MemberInfo memberInfo, int maxSize) : base(modifiedType, memberInfo)
			{
				_maxSize = maxSize;
				_canBeNull = !memberInfo.GetMemberType().GetElementType().IsValueType;
			}

			private readonly bool _canBeNull;
			private readonly int _maxSize;

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				if (array[i++] is int len)
				{
					if (len > _maxSize || len < 0) return false;

					var desArray = new object[len];

					if (array.Length <= (i - 1) + len) return false;

					for (var desArrayIndex = 0; desArrayIndex < len; desArrayIndex++)
					{
						if (!(array[i] is T || (_canBeNull && array[i] == null)))
						{
							return false;
						}

						desArray[desArrayIndex] = array[i++];
					}

					SetValue(instance, desArray);

					return true;
				}

				return false;
			}

			public override void Serialize(object instance, ref object[] array, ref int i)
			{
				var arrayVal = (T[])GetValue(instance);
				array[i++] = arrayVal.Length;

				for (var arrayValInex = 0; arrayValInex < arrayVal.Length; arrayValInex++)
				{
					array[i++] = arrayVal[arrayValInex];
				}
			}

			public override void EstimateSize(object instance, ref int i)
				=> i += ((T[])GetValue(instance)).Length + 1;
		}
	}
}

/*using SwissILKnife;

using System;
using System.Reflection;

namespace Decorator
{
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class ArrayAttribute : Attribute, IDecoratorInfoAttribute
{
	public ArrayAttribute() : this(0xFFFF)
	{
	}

	public ArrayAttribute(int maxArraySize)
		=> MaxArraySize = maxArraySize;

	public int MaxArraySize {get; set;}

	DecoratorInfo IDecoratorInfoAttribute.GetDecoratorInfo(MemberInfo memberValue)
	{
		return Make.Class(typeof(Array<>)).Generic(memberValue.GetMemberType().GetElementType())
				.CreateDecoratorInfo(memberValue, MaxArraySize);
	}
}

internal class Array<T> : DecoratorInfo<T>
{
	public Array(MemberInfo memberInfo, int maxSize) : base(memberInfo)
	{
		_maxSize = maxSize;
		_canBeNull = !memberInfo.GetMemberType().GetElementType().IsValueType;
	}

	private readonly bool _canBeNull;
	private readonly int _maxSize;

	public override bool Deserialize(object instance, ref object[] array, ref int i)
	{
		if (array[i++] is int len)
		{
			if (len > _maxSize || len < 0) return false;

			var desArray = new object[len];

			if (array.Length <= (i - 1) + len) return false;

			for (var desArrayIndex = 0; desArrayIndex < len; desArrayIndex++)
			{
				if (!(array[i] is T || (_canBeNull && array[i] == null)))
				{
					return false;
				}

				desArray[desArrayIndex] = array[i++];
			}

			SetValue(instance, desArray);

			return true;
		}

		return false;
	}

	public override void Serialize(object instance, ref object[] array, ref int i)
	{
		var arrayVal = (T[])GetValue(instance);
		array[i++] = arrayVal.Length;

		for (var arrayValInex = 0; arrayValInex < arrayVal.Length; arrayValInex++)
		{
			array[i++] = arrayVal[arrayValInex];
		}
	}

	public override void EstimateSize(object instance, ref int i)
		=> i += ((T[])GetValue(instance)).Length + 1;
}
}*/