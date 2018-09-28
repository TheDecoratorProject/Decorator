namespace Decorator.Helpers
{
	internal static class TryMethodHelpers
	{
		public static bool EndTryMethod<T>(bool result, T value, out T setDefault)
		{
			setDefault = value;
			return result;
		}
	}
}