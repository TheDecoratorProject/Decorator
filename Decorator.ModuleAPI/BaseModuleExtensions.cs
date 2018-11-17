﻿namespace Decorator.ModuleAPI
{
	public static class BaseModuleExtensions
	{
		public static int EstimateSize<T>(this BaseModule[] decoratorModules, T item)
		{
			var estimateSize = 0;

			for (var memberIndex = 0; memberIndex < decoratorModules.Length; memberIndex++)
			{
				decoratorModules[memberIndex].EstimateSize(item, ref estimateSize);
			}

			return estimateSize;
		}
	}
}