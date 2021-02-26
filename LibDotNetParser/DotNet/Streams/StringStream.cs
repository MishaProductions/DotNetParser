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
}
