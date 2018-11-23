using System;
using System.Reflection.Emit;

namespace Decorator.ModuleAPI.IL
{
	public class ILSerializeMethodContainer
	{
		public ILSerializeMethodContainer(LocalBuilder index, Action loadMemberValue, Action<Action> setArrayValue)
		{
			Index = index;
			LoadMemberValue = loadMemberValue;
			SetArrayValue = setArrayValue;
		}

		public LocalBuilder Index { get; }
		public Action LoadMemberValue { get; }
		public Action<Action> SetArrayValue { get; }
	}
}