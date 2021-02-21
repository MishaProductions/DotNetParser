using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibDotNetParser.DotNet.Streams
{
    /// <summary>
    /// #USer Stream
    /// </summary>
    public class USStream
    {
        private readonly Dictionary<uint, string> _strings;

        public USStream(Dictionary<uint, string> strings)
        {
            _strings = strings;
        }

        public string GetByOffset(uint offset)
        {
            if (!_strings.ContainsKey(offset))
                return "<BUG> No string at offset: " + offset;
            return _strings[offset];
        }

        public IEnumerable<string> GetAll()
        {
            return _strings.Values;
        }
    }

    public class USStreamReader
    {
        private readonly BinaryReader _reader;
        private readonly int _dataSize;

        public USStreamReader(byte[] data)
        {
            _dataSize = data.Length;
            _reader = new BinaryReader(new MemoryStream(data));
        }

        public USStream Read()
        {
            var strings = new Dictionary<uint, string>();
            uint CurrentString = 1;
            //The US Stream starts with a null byte, so skip it
            for (int i = 1; i < _dataSize; i++)
            {
                //First comes the string length
                byte len = _reader.ReadByte();

                i++;
                //If the length is zero, then ignore
                if (len == 0)
                    continue;

                //Read the bytes of that string
                string s = "";
                for (int i2 = 0; i2 < len; i2++)
                {
                    char x = _reader.ReadChar();
                    if (x != '\0')
                        s += x;
                }

                //Add it to the Dictionary
                strings.Add(CurrentString, s);
                //Continue
                i = (int)_reader.BaseStream.Position;
                CurrentString++;
            }
            return new USStream(strings);
        }
    }
}
