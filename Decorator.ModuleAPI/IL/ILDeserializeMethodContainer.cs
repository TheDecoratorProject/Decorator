using System;
using System.Collections.Generic;
using System.Text;

namespace Decorator.ModuleAPI.IL
{
	public class ILDeserializeMethodContainer
	{
		public ILDeserializeMethodContainer(Action loadMemberValue, Action<Action> setMemberValue, Action loadCurrentObject, Action loadIndex, Action<Action> addToIndex)
		{
			LoadMemberValue = loadMemberValue;
			SetMemberValue = setMemberValue;
			LoadCurrentObject = loadCurrentObject;
			LoadIndex = loadIndex;
			AddToIndex = addToIndex;
		}

		public Action LoadMemberValue { get; }
		public Action<Action> SetMemberValue { get; }
		public Action LoadCurrentObject { get; }
		public Action LoadIndex { get; }
		public Action<Action> AddToIndex { get; }
	}
}