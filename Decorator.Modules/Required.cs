using Decorator.ModuleAPI;
using Decorator.ModuleAPI.IL;
using StrictEmit;

using System;
using System.Reflection.Emit;

namespace Decorator.Modules
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class RequiredAttribute : Attribute, IModuleAttribute
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public Module<T> Build<T>(BaseContainer modContainer)
			=> typeof(T).IsValueType ?
				new RequiredModule<T>(modContainer)
				: (Module<T>)new RequiredModule<T>(modContainer);

		// TODO: Combine these
		// TODO: Comment on what the IL so i can actually read it

		public class RequiredModule<T> : Module<T>, ILSupport
		{
			public RequiredModule(BaseContainer modContainer)
				: base(modContainer)
			{
				_valueType = modContainer.Member.MemberType.IsValueType;
			}

			private bool _valueType;

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				object value = array[i++];

				if (value is T || 
					(_valueType ? false : value == null))
				{
					SetValue(instance, value);
					return true;
				}

				i--;

				return false;
			}

			public override void EstimateSize(object instance, ref int size) => size++;

			public override void Serialize(object instance, ref object[] array, ref int i) => array[i++] = GetValue(instance);

			public void GenerateDeserialize(ILGenerator il, ILDeserializeMethodContainer ilMethods)
			{
				var local = il.DeclareLocal(typeof(object));
				var isntIt = il.DefineLabel();

				// object objVal = array[i];
				ilMethods.LoadCurrentObject();
				il.EmitSetLocalVariable(local);

				// if (!(objVal is T))
				il.EmitLoadLocalVariable(local);
				il.EmitIsInstance<T>();
				il.EmitShortBranchTrue(isntIt);

				// if it's not a value type, we can check if it's null (since reference types can be null)
				if (!_valueType)
				{
					// || objVal == null))
					il.EmitLoadLocalVariable(local);
					il.EmitShortBranchFalse(isntIt);
				}

				// return false;
				il.EmitConstantInt(0);
				il.EmitReturn();

				il.MarkLabel(isntIt);

				// i++;
				ilMethods.AddToIndex(() => il.EmitConstantInt(1));

				// result.Property = (T)objVal;
				ilMethods.SetMemberValue(() =>
				{
					il.EmitLoadLocalVariable(local);
				});
			}

			public void GenerateEstimateSize(ILGenerator il, ILEstimateSizeMethodContainer ilMethods)
			{
				ilMethods.AddToSize(() => il.EmitConstantInt(1));
			}

			public void GenerateSerialize(ILGenerator il, ILSerializeMethodContainer ilMethods)
			{
				ilMethods.SetArrayValue(ilMethods.LoadMemberValue);

				ilMethods.AddToIndex(() => il.EmitConstantInt(1));
			}
		}
	}
}
 
 