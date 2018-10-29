using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decorator.Serialize.Serializers
{
    public class IntegerSerializer : ISerializer<int>
    {
        public bool Supports(object item) => item is int;

        public void Write(object item, Stream writeTo) => Write((int)item, writeTo);
        object ISerializer.Read(Stream readFrom) => Read(readFrom);

        public void Write(int item, Stream writeTo)
            => writeTo.Write(BitConverter.GetBytes(item), 0, 4);

        public int Read(Stream readFrom)
        {
            var data = new byte[4];
            readFrom.Read(data, 0, 4);
            return BitConverter.ToInt32(data, 0);
        }
    }
}
