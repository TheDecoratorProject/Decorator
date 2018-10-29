using System;
using System.IO;

namespace Decorator.Serialize
{
    public interface ISerializer
    {
        bool Supports(object item);

        void Write(object item, Stream writeTo);
        object Read(Stream readFrom);
    }

    public interface ISerializer<T> : ISerializer
    {
        void Write(T item, Stream writeTo);
        T Read(Stream readFrom);
    }
}
