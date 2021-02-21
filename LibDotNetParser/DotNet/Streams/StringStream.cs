using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LibDotNetParser.DotNet.Streams
{
    public class StringsStream
    {
        private readonly Dictionary<uint, string> _strings;

        public StringsStream(Dictionary<uint, string> strings)
        {
            _strings = strings;
        }

        public string GetByOffset(uint offset)
        {
            if (!_strings.ContainsKey(offset))
                return "<BUG> No string at offset: "+offset;
            return _strings[offset];
        }

        public IEnumerable<string> GetAll()
        {
            return _strings.Values;
        }
    }

    public class StringsStreamReader
    {
        private readonly BinaryReader _reader;
        private readonly int _dataSize;

        public StringsStreamReader(byte[] data)
        {
            _dataSize = data.Length;
            _reader = new BinaryReader(new MemoryStream(data));
        }

        public StringsStream Read()
        {
            var strings = new Dictionary<uint, string>();
            while (_reader.BaseStream.Position < _dataSize)
            {
                strings.Add((uint)_reader.BaseStream.Position, _reader.ReadNullTermString());
            }
            return new StringsStream(strings);
        }
    }

    public static class Ext
    {
        public static string ReadNullTermString(this BinaryReader reader,bool secondPass=false)
        {
            var buffer = new List<char>();
            char current;
            while ((current = reader.ReadChar()) != '\0')
                buffer.Add(current);

            if (secondPass)
                while ((current = reader.ReadChar()) != '\0')
                    buffer.Add(current);
            return new string(buffer.ToArray());
        }
        public static string ReadNullTermString(this BinaryReader reader, int readLength)
        {
            var bytes = reader.ReadChars(readLength);
            List<char> b = new List<char>();
            foreach (var item in bytes)
            {
                if (!item.Equals('\0'))
                {
                    b.Add(item);
                }
                else
                {
                    break;
                }
            }
            return new string(b.ToArray());
        }
    }
}
