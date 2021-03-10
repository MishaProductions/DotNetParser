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
            //Don't ask me how this works becuase idk
            var strings = new Dictionary<uint, string>();
            uint CurrentString = 1;
            _reader.BaseStream.Position = 1;

            //The US Stream starts with a null byte, so skip it
            for (int i = 1; i < _dataSize; i++)
            {
                var lenNotProper = _reader.ReadByte();
                _reader.BaseStream.Position--;

                var len = Read7BitInt(_reader);
                var str = "";

                if (len == 0) //Ignore zero sized strings
                    continue;

                for (int i2 = 0; i2 < len; i2++)
                {
                    try
                    {
                        char c = _reader.ReadChar();
                        if (c != '\0')
                            str += c;
                    }
                    catch (System.IO.EndOfStreamException)
                    {

                    }
                }

                i = (int)_reader.BaseStream.Position;

                if (len % 2 == 0)
                {
                    //When string is even, there is an additional null byte
                    i++;
                    _reader.BaseStream.Position++;
                }
                else
                {
                    //is odd
                }
                var x =  i - lenNotProper - 1;// - _dataSize;


                strings.Add((uint)x, str);

                CurrentString++;
            }
            return new USStream(strings);
        }

        private int Read7BitInt(BinaryReader r)
        {
            int size = 0;
            int temp = 0;

            while (true)
            {
                temp += r.ReadByte() & 0x7F;
                size++;

                try
                {
                    var b = r.ReadByte();
                    if (b > 127)
                    {
                        temp <<= 7;
                    }
                    else
                    {
                        r.BaseStream.Position -= size;
                        break;
                    }
                }
                catch(EndOfStreamException)
                {
                    r.BaseStream.Position -= size;
                    return temp;
                }
            }

            return temp;
        }
    }
}
