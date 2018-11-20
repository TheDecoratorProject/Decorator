using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Decorator.ModuleAPI
{
	public interface ILSupport
	{
		void GenerateDeserialize(ILGenerator il, Action loadMemberValue, Action<Action> setMemberValue, Action loadValue, Action loadI, Action<int> addToI);

		// void GenerateSerialize(ILGenerator il, Action loadMemberValue, Action<Action> setMemberValue, Action setToArray);
	}
}
