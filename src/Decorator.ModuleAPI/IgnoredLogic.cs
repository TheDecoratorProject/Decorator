using System.Reflection.Emit;
using Decorator.ModuleAPI.IL;
using StrictEmit;

namespace Decorator.ModuleAPI
{
	public class IgnoredLogic : BaseModule, ILSupport
	{
		public override BaseContainer ModuleContainer => default;

		public override bool Deserialize(object instance, ref object[] array, ref int i)
		{
			i++;
			return true;
		}

		public override void Serialize(object instance, ref object[] array, ref int i) => i++;

		public override void EstimateSize(object instance, ref int i) => i++;

		public void GenerateDeserialize(ILGenerator il, ILDeserializeMethodContainer ilMethods)
		{
			ilMethods.AddToIndex(() => il.EmitConstantInt(1));
		}

		public void GenerateEstimateSize(ILGenerator il, ILEstimateSizeMethodContainer ilMethods)
		{
			ilMethods.AddToSize(() => il.EmitConstantInt(1));
		}

		public void GenerateSerialize(ILGenerator il, ILSerializeMethodContainer ilMethods)
		{
			ilMethods.AddToIndex(() => il.EmitConstantInt(1));
		}
	}
}