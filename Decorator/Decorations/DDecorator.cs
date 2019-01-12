using System.Runtime.CompilerServices;

namespace Decorator
{
	// the static
	public class DDecorator<T>
	{
		public static Decorator<T> Instance { get; } =
			new Decorator<T>
			(
				Compiler<T>.Compile
				(
					new Discovery<T>(),
					new DecorationFactoryBuilder()
				)
			);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object[] Serialize(T item) => Instance.Serialize(item);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object[] Serialize(T item, ref int index) => Instance.Serialize(item, ref index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryDeserialize(object[] array, out T result) => Instance.TryDeserialize(array, out result);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryDeserialize(object[] array, ref int index, out T result) => Instance.TryDeserialize(array, ref index, out result);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryDeserialize(object[] array, T instance, ref int index) => Instance.TryDeserialize(array, instance, ref index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int EstimateSize(T instance) => Instance.EstimateSize(instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void EstimateSize(T instance, ref int index) => Instance.EstimateSize(instance, ref index);
	}
}