using Decorator.Serialize.Serializers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Decorator.Serialize
{
    public class Serializer
    {
        public Serializer()
        {
            _serializerCache = new ConcurrentDictionary<int, int>();
            _serializers = new List<ISerializer>();
            _lock = new object();

            _nullSer = new NullSerializer();
        }

        private object _lock;
        private List<ISerializer> _serializers;
        private INullSerializer _nullSer;
        private ConcurrentDictionary<int, int> _serializerCache;

        // <sum> recommended
        public void RegisterInstance<T>(ISerializer<T> serializer)
        {
            lock (_lock)
            {
                if(_serializers.Count >= byte.MaxValue)
                {
                    //TODO: throw exception
                }

                foreach (var i in _serializers)
                {
                    if (i is ISerializer<T>)
                    {
                        //TODO: throw exception
                    }
                }

                var pos = RegisterInstance_Internal(serializer);

                if (!_serializerCache.TryAdd(typeof(T).GetHashCode(), pos))
                {
                    //TODO: throw exception
                }
            }
        }

        // <sum> bit more unsafe
        public void RegisterInstance(ISerializer serializer)
        {
            if (serializer is INullSerializer nullSerializer)
            {
                _nullSer = nullSerializer;
            }
            else
            {
                lock (_lock)
                {
                    RegisterInstance_Internal(serializer);
                }
            }
        }

        private int RegisterInstance_Internal(ISerializer serializer)
        {
            _serializers.Add(serializer);
            return _serializers.Count - 1;
        }

        public void Save<T>(T data, Stream writeTo)
        {
            //TODO: call decorator
        }

        public void Save(object[] data, Stream writeTo)
        {
            var _id = new byte[1];

            foreach(var i in data)
            {
                using (var ms = new MemoryStream(0xF))
                {
                    var serializer = FindSerializerFor(i, ref _id);

                    serializer.Write(i, ms);

                    writeTo.Write(_id, 0, 1);
                    // TODO: write len
                }
            }
        }

        private ISerializer FindSerializerFor(object info, ref byte[] id)
        {
            if (info == null)
            {
                id[0] = 0;

                return _nullSer;
            }

            id[0] = 1;
            foreach (var i in _serializers)
            {
                if (i.Supports(info))
                {
                    return i;
                }

                id[0]++;
            }

            return default;
            //TODO: throw exception
        }
    }
}
