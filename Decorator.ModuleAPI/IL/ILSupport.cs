using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Decorator.ModuleAPI.IL
{
	// TODO: xml doc on everyone

	public interface ILSupport
	{
		void GenerateDeserialize(ILGenerator il, ILDeserializeMethodContainer ilMethods);

		void GenerateEstimateSize(ILGenerator il, ILEstimateSizeMethodContainer ilMethods);
		void GenerateSerialize(ILGenerator il, ILSerializeMethodContainer ilMethods);
	}
}