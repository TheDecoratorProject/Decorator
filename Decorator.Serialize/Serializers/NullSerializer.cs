using System.IO;

namespace Decorator.Serialize.Serializers
{
	public class NullSerializer : INullSerializer
	{
		public bool Supports(object item) => item == null;

		public object Read(Stream readFrom) => null;

		public void Write(object item, Stream writeTo) => writeTo.Write(new byte[] { 0xFF }, 0, 1);
	}
}