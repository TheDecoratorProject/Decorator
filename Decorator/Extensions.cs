namespace Decorator
{
	internal static class Extensions
	{
		public static int EstimateSize<T>(this DecoratorInfo[] decoratorInfos, T item)
		{
			int estimateSize = 0;

			for (int memberIndex = 0; memberIndex < decoratorInfos.Length; memberIndex++)
			{
				decoratorInfos[memberIndex].EstimateSize(item, ref estimateSize);
			}

			return estimateSize;
		}
	}
}