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
				addToI(1);

				// result.Property = (T)objVal;
				setMemberValue(() =>
				{
					il.EmitLoadLocalVariable(local);

					if(!_valueType)
					{
						il.EmitCastClass(typeof(T));
					}
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
		/*
		public class RequiredModule<T> : Module<T>, ILSupport
		{
			public RequiredModule(BaseContainer modContainer)
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
		}*/
	}
}
 
 