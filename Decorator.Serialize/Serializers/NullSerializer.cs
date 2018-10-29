using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decorator.Serialize.Serializers
{
    public class NullSerializer : INullSerializer
    {
        public bool Supports(object item) => item == null;

        public object Read(Stream readFrom) => null;
        public void Write(object item, Stream writeTo) => writeTo.Write(new byte[1] { 0xFF }, 0, 1);
    }
}
