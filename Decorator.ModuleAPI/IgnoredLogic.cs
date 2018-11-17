using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator.ModuleAPI
{
	public class IgnoredLogic : BaseModule
	{
		public override BaseContainer ModuleContainer => default;

		public override bool Deserialize(object instance, ref object[] array, ref int i)
		{
			i++;
			return true;
		}

		public override void Serialize(object instance, ref object[] array, ref int i) => i++;

		public override void EstimateSize(object instance, ref int i) => i++;
	}
}
