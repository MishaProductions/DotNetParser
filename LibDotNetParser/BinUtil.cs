using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibDotNetParser
{
    public static class BinUtil
    {
        public static byte ConvertBoolArrayToByte(this bool[] source)
        {
            byte result = 0;
            // This assumes the array never contains more than 8 elements!
            int index = 8 - source.Length;

            // Loop through the array
            foreach (bool b in source)
            {
                // if the element is 'true' set the bit at that position
                if (b)
                    result |= (byte)(1 << (7 - index));

                index++;
            }

            return result;
        }

        public static bool[] ConvertByteToBoolArray(this byte b)
        {
            // prepare the return result
            bool[] result = new bool[8];

            // check each bit in the byte. if 1 set to true, if 0 set to false
            for (int i = 0; i < 8; i++)
                result[i] = (b & (1 << i)) == 0 ? false : true;

            // reverse the array
            Array.Reverse(result);

            return result;
        }

        public static string ReadNullTermString(this BinaryReader reader)
        {
            var buffer = new List<char>();
            char current;
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
