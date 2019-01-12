using System;

namespace Decorator.ModuleAPI.IL
{
	public class ILEstimateSizeMethodContainer
	{
		public ILEstimateSizeMethodContainer(Action loadCurrentObject, Action<Action> addToSize)
		{
			LoadCurrentObject = loadCurrentObject;
			AddToSize = addToSize;
		}

		public Action LoadCurrentObject { get; }
		public Action<Action> AddToSize { get; }
	}
}