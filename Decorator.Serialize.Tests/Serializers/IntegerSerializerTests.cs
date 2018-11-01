using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Decorator.Serialize.Serializers;
using Xunit;

namespace Decorator.Serialize.Tests.Serializers
{
    public class IntegerSerializerTests
    {
		private readonly IntegerSerializer _ser;

		public IntegerSerializerTests()
        {
            _ser = new IntegerSerializer();
        }

        [Fact]
        public void CanWrite()
        {
            const int data = 1234;

            using (var ms = new MemoryStream())
            {
                _ser.Write(data, ms);

                ms.Seek(0, SeekOrigin.Begin);

                var bytes = new byte[4];

                ms.Read(bytes, 0, 4);

                Assert.Equal(data, BitConverter.ToInt32(bytes));
            }
        }

        [Fact]
        public void CanRead()
        {
            const int data = 1234;

            using (var ms = new MemoryStream())
            {
                _ser.Write(data, ms);

                ms.Seek(0, SeekOrigin.Begin);
                
                Assert.Equal(data, _ser.Read(ms));
            }
        }

        [Fact]
        public void ValidInts()
        {
            Assert.True(_ser.Supports(123));
            Assert.True(_ser.Supports(456));

            Assert.False(_ser.Supports(123L));
            Assert.False(_ser.Supports(123u));
            Assert.False(_ser.Supports(""));
        }

        [Fact]
        public void ReadWriteObject()
        {
            object data = 1234;

            using (var ms = new MemoryStream())
            {
                _ser.Write(data, ms);

                ms.Seek(0, SeekOrigin.Begin);

                Assert.Equal(data, _ser.Read(ms));
            }
        }
    }
}
