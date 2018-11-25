using Decorator.ModuleAPI;
using Decorator.ModuleAPI.IL;
using StrictEmit;
using System;
using System.Reflection.Emit;

namespace Decorator.Modules
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class OptionalAttribute : Attribute, IModuleAttribute
	{
		public Type ModifyAppliedType(Type attributeAppliedTo)
			=> attributeAppliedTo;

		public Module<T> Build<T>(BaseContainer modContainer)
			=> new OptionalModule<T>(modContainer);

		public class OptionalModule<T> : Module<T>, ILSupport
		{
			public OptionalModule(BaseContainer modContainer)
				: base(modContainer)
			{
				_valueType = typeof(T).IsValueType;
			}

			private readonly bool _valueType;

			public override bool Deserialize(object instance, ref object[] array, ref int i)
			{
				if (array[i] is T ||
					(!_valueType &&
					array[i] == null))
				{
					SetValue(instance, array[i]);
				}

				i++;

				return true;
			}

			public override void Serialize(object instance, ref object[] array, ref int i) => array[i++] = GetValue(instance);

			public override void EstimateSize(object instance, ref int i) => i++;

			public void GenerateDeserialize(ILGenerator il, ILDeserializeMethodContainer ilMethods)
			{
				var local = il.DeclareLocal(typeof(object));
				var isInst = il.DeclareLocal(typeof(T));

				ilMethods.LoadCurrentObject();
				il.EmitSetLocalVariable(local);

				if (_valueType)
				{
					// value type
					/*
					 * object local = array[i];
					 * 
					 * if (local is T isBranchSet)
					 * {
					 *     result.Member = isBranchSet;
					 * }
					 */

					var ifCompleted = il.DefineLabel();

					// if (local is T obj4)
					il.EmitLoadLocalVariable(local);
					il.EmitIsInstance<T>();
					il.EmitShortBranchFalse(ifCompleted);

					il.EmitLoadLocalVariable(local);
					il.EmitSetLocalVariable(isInst);
					ilMethods.SetMemberValue(() =>
					{
						il.EmitLoadLocalVariable(isInst);
					});

					il.MarkLabel(ifCompleted);

					ilMethods.AddToIndex(() => il.EmitConstantInt(1));
				}
				else
				{
					// reference type
					/*
					 * object local = array[i];
					 * 
					 * if (local is T isBranchSet)
					 * {
					 *     result.Member = isBranchSet;
					 * }
					 * else if (local == null)
					 * {
					 *     result.Member = null;
					 * }
					 */

					var ifCompleted = il.DefineLabel();
					var secondBranch = il.DefineLabel();

					// if (local is T isInst)
					il.EmitLoadLocalVariable(local);
					il.EmitIsInstance<T>();
					il.EmitDuplicate();
					il.EmitSetLocalVariable(isInst);
					il.EmitShortBranchFalse(secondBranch);

					// result.Member = isInst;
					ilMethods.SetMemberValue(() =>
					{
						il.EmitLoadLocalVariable(isInst);
					});

					il.EmitBranch(ifCompleted);

					il.MarkLabel(secondBranch);

					// else if (local == null)
					il.EmitLoadLocalVariable(local);
					il.EmitShortBranchTrue(ifCompleted);

					// result.Member = null;
					ilMethods.SetMemberValue(() =>
					{
						il.EmitLoadNull();
					});

					// }
					il.MarkLabel(ifCompleted);

					ilMethods.AddToIndex(() => il.EmitConstantInt(1));
				}
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