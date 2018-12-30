using System;
using System.IO;

namespace Decorator.Serialize.Serializers
{
	public class IntegerSerializer : ISerializer<int>
	{
		private const int IntSize = sizeof(int);

		public bool Supports(object item) => item is int;

		public void Write(object item, Stream writeTo) => Write((int)item, writeTo);

		object ISerializer.Read(Stream readFrom) => Read(readFrom);

		public void Write(int item, Stream writeTo)
			=> writeTo.Write(BitConverter.GetBytes(item), 0, IntSize);

		public int Read(Stream readFrom)
		{
			var data = new byte[IntSize];

			if (readFrom.Read(data, 0, IntSize) != IntSize)
			{
				// TODO: throw exception
			}

			return BitConverter.ToInt32(data, 0);
		}
	}
}