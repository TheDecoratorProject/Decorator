using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Decorator.ModuleAPI
{
	public interface ILSupport
	{
		void GenerateDeserialize(ILGenerator il, Action loadMemberValue, Action<Action> setMemberValue, Action loadValue, Action loadI, Action<int> addToI);

		void GenerateSerializeSize(ILGenerator il, Action loadValue, Action<Action> addValue);
		void GenerateSerialize(ILGenerator il, LocalBuilder index, Action loadMemberValue, Action<Action> setArrayValue);
	}
}
