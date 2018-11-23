using System;
using System.Reflection.Emit;

namespace Decorator.ModuleAPI.IL
{
	public class ILSerializeMethodContainer
	{
		public ILSerializeMethodContainer(LocalBuilder index, Action loadMemberValue, Action<Action> setArrayValue, Action<Action> addToIndex)
		{
			Index = index;
			LoadMemberValue = loadMemberValue;
			SetArrayValue = setArrayValue;
			AddToIndex = addToIndex;
		}

		public LocalBuilder Index { get; }
		public Action LoadMemberValue { get; }
		public Action<Action> SetArrayValue { get; }
		public Action<Action> AddToIndex { get; }
	}
}