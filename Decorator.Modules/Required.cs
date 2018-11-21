using Decorator.ModuleAPI;

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
				new RequiredValueTypeModule<T>(modContainer)
				: (Module<T>)new RequiredReferenceTypeModule<T>(modContainer);

		public class RequiredValueTypeModule<T> : Module<T>, ILSupport
		{
			public RequiredValueTypeModule(BaseContainer modContainer)
				: base(modContainer)
			{
			}

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				object value = array[i++];

				if (!(value is T))
				{
					i--;

					return false;
				}

				SetValue(instance, value);

				return true;
			}

			public override void EstimateSize(object instance, ref int size) => size++;

			public override void Serialize(object instance, ref object[] array, ref int i) => array[i++] = GetValue(instance);

			public void GenerateDeserialize(ILGenerator il, Action loadMemberValue, Action<Action> setMemberValue, Action loadValue, Action loadI, Action<int> addToI)
			{
				var local = il.DeclareLocal(typeof(object));
				var isntIt = il.DefineLabel();

				// object objVal = array[i];
				loadValue();

				il.EmitSetLocalVariable(local);

				// if (!(objVal is T))
				il.EmitLoadLocalVariable(local);
				il.EmitIsInstance<T>();
				il.EmitShortBranchTrue(isntIt);

				// return false;
				il.EmitConstantInt(0);
				il.EmitReturn();

				il.MarkLabel(isntIt);

				// i++;
				addToI(1);

				// result.Property = (T)objVal;
				setMemberValue(() =>
				{
					il.EmitLoad(typeof(object));
					il.EmitLoadLocalVariable(local);
				});
			}

			public void GenerateSerializeSize(ILGenerator il, Action loadValue, Action<Action> addValue)
			{
				addValue(() => il.EmitConstantInt(1));
			}

			public void GenerateSerialize(ILGenerator il, LocalBuilder index, Action loadMemberValue, Action<Action> setArrayValue)
			{
				setArrayValue(loadMemberValue);

				il.EmitLoadLocalVariable(index);
				il.EmitConstantInt(1);
				il.EmitAdd();
				il.EmitSetLocalVariable(index);
			}
		}

		public class RequiredReferenceTypeModule<T> : Module<T>, ILSupport
		{
			public RequiredReferenceTypeModule(BaseContainer modContainer)
				: base(modContainer)
			{
			}

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				object value = array[i++];

				if (value is T ||
					value == null)
				{
					SetValue(instance, value);
					return true;
				}

				i--;

				return false;
			}

			public override void EstimateSize(object instance, ref int size) => size++;
			public override void Serialize(object instance, ref object[] array, ref int i) => array[i++] = GetValue(instance);

			public void GenerateDeserialize(ILGenerator il, Action loadMemberValue, Action<Action> setMemberValue, Action loadValue, Action loadI, Action<int> addToI)
			{
				var local = il.DeclareLocal(typeof(object));
				var isntIt = il.DefineLabel();

				// object objVal = array[i];
				loadValue();

				il.EmitSetLocalVariable(local);

				// if (!(objVal is T
				il.EmitLoadLocalVariable(local);
				il.EmitIsInstance<T>();
				il.EmitShortBranchTrue(isntIt);

				// || objVal == null))
				il.EmitLoadLocalVariable(local);
				il.EmitShortBranchFalse(isntIt);

				// return false;
				il.EmitConstantInt(0);
				il.EmitReturn();

				il.MarkLabel(isntIt);

				// i++;
				addToI(1);

				// result.Property = (T)objVal;
				setMemberValue(() =>
				{
					il.EmitLoad(typeof(object));
					il.EmitLoadLocalVariable(local);
					il.EmitCastClass(typeof(T));
				});
			}

			public void GenerateSerializeSize(ILGenerator il, Action loadValue, Action<Action> addValue)
			{
				addValue(() => il.EmitConstantInt(1));
			}

			public void GenerateSerialize(ILGenerator il, LocalBuilder index, Action loadMemberValue, Action<Action> setArrayValue)
			{
				setArrayValue(loadMemberValue);

				il.EmitLoadLocalVariable(index);
				il.EmitConstantInt(1);
				il.EmitAdd();
				il.EmitSetLocalVariable(index);
			}
		}
	}
}
 
 