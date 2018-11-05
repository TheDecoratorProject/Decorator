using Decorator.ModuleAPI;
using SwissILKnife;
using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class FlattenArrayAttribute : Attribute, IDecoratorModuleBuilder, IOnlyDecorablesDecorableModuleBuilder
	{
		public FlattenArrayAttribute() : this(0xFFFF)
		{
		}

		public FlattenArrayAttribute(int arraySize)
			=> MaxArraySize = arraySize;

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
		{
			return EnsureIDecorable<FlattenArrayAttribute>.InvokeBuild<T>(this, modifiedType, memberInfo);
		}

		public DecoratorModule<T> BuildDecorable<T>(Type modifiedType, MemberInfo memberInfo)
			where T : IDecorable
		{
			return new Module<T>(modifiedType, memberInfo, MaxArraySize);
		}

		public class Module<T> : DecoratorModule<T>
			where T : IDecorable
		{
			public Module(Type modifiedType, MemberInfo memberInfo, int arraySize)
				: base(modifiedType, memberInfo)
			{
				_maxSize = arraySize;
			}

			private int _maxSize;

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				if (array[i++] is int len)
				{
					if (len > _maxSize || len < 0) return false;

					var desArray = new object[len];

					for (var desArrayIndex = 0; desArrayIndex < len; desArrayIndex++)
					{
						if (!Converter<T>.TryDeserialize(array, ref i, out var item))
						{
							return false;
						}

						desArray[desArrayIndex] = item;
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

				for (var arrayValIndex = 0; arrayValIndex < arrayVal.Length; arrayValIndex++)
				{
					var data = Converter<T>.Serialize(arrayVal[arrayValIndex]);

					for (var arrayIndex = 0; arrayIndex < data.Length; arrayIndex++)
					{
						array[i++] = data[arrayIndex];
					}
				}
			}

			public override void EstimateSize(object instance, ref int i)
			{
				var array = (T[])GetValue(instance);

				i++;

				for (var arrayIndex = 0; arrayIndex < array.Length; arrayIndex++)
				{
					i += DecoratorInfoContainer<T>.Members.EstimateSize(array[arrayIndex]);
				}
			}
		}
	}
}

/*using SwissILKnife;

using System;
using System.Reflection;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class FlattenArrayAttribute : Attribute, IDecoratorInfoAttribute
	{
		public FlattenArrayAttribute() : this(0xFFFF)
		{
		}

		public FlattenArrayAttribute(int maxArraySize)
			=> MaxArraySize = maxArraySize;

		public int MaxArraySize { get; set; }

		DecoratorInfo IDecoratorInfoAttribute.GetDecoratorInfo(MemberInfo memberValue)
		{
			return Make.Class(typeof(FlattenArray<>)).Generic(memberValue.GetMemberType().GetElementType())
					.CreateDecoratorInfo(memberValue, MaxArraySize);
		}
	}

	internal class FlattenArray<T> : DecoratorInfo
		where T : IDecorable
	{
		public FlattenArray(MemberInfo memberInfo, int maxSize)
		{
			_maxSize = maxSize;
			_getValue = MemberUtils.GetGetMethod(memberInfo);
			_setValue = MemberUtils.GetSetMethod(memberInfo);
		}

		protected Func<object, object> _getValue;
		protected Action<object, object> _setValue;
		private readonly int _maxSize;

		public override bool Deserialize(object instance, ref object[] array, ref int i)
		{
			if (array[i++] is int len)
			{
				if (len > _maxSize || len < 0) return false;

				var desArray = new object[len];

				for (var desArrayIndex = 0; desArrayIndex < len; desArrayIndex++)
				{
					if (!Converter<T>.TryDeserialize(array, ref i, out var item))
					{
						return false;
					}

					desArray[desArrayIndex] = item;
				}

				_setValue(instance, desArray);

				return true;
			}

			return false;
		}

		public override void Serialize(object instance, ref object[] array, ref int i)
		{
			var arrayVal = (T[])_getValue(instance);

			array[i++] = arrayVal.Length;

			for (var arrayValIndex = 0; arrayValIndex < arrayVal.Length; arrayValIndex++)
			{
				var data = Converter<T>.Serialize(arrayVal[arrayValIndex]);

				for (var arrayIndex = 0; arrayIndex < data.Length; arrayIndex++)
				{
					array[i++] = data[arrayIndex];
				}
			}
		}

		public override void EstimateSize(object instance, ref int i)
		{
			var array = (T[])_getValue(instance);

			i++;

			for (var arrayIndex = 0; arrayIndex < array.Length; arrayIndex++)
			{
				i += DecoratorInfoContainer<T>.Members.EstimateSize(array[arrayIndex]);
			}
		}
	}
}*/
