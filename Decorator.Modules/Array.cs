using Decorator.ModuleAPI;

using System;

namespace Decorator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ArrayAttribute : Attribute, IModuleBuilder
	{
		public ArrayAttribute() : this(ushort.MaxValue)
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

		public Module<T> Build<T>(BaseContainer modContainer)
			=> new ArrayModule<T>(modContainer, MaxArraySize);

		public class ArrayModule<T> : Module<T>
		{
			public ArrayModule(BaseContainer modContainer, int maxSize) : base(modContainer)
			{
				_maxSize = maxSize;
				_canBeNull = !typeof(T).IsValueType;
			}

			private readonly bool _canBeNull;
			private readonly int _maxSize;

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				if (!(array[i] is int len))
				{
					return false;
				}

				if (len > _maxSize || len < 0 ||
					(array.Length <= i + len))
				{
					return false;
				}

				var desArray = new object[len];

				i++;

				for (var desArrayIndex = 0; desArrayIndex < len; desArrayIndex++)
				{
					if (!(array[i] is T ||
						(_canBeNull && array[i] == null)))
					{
						return false;
					}

					desArray[desArrayIndex] = array[i];

					i++;
				}

				SetValue(instance, desArray);

				return true;
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