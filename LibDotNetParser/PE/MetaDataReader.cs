using System.IO;
using System.Text;

namespace LibDotNetParser.PE
{
    public class MetadataReader : BinaryReader
    {
        public StreamOffsetSizeFlags StreamOffsetSizeFlags { get; set; }

        public MetadataReader(Stream input) : base(input)
        {

        }

        public MetadataReader(Stream input, Encoding encoding)
            : base(input, encoding)
        {
        }

        public uint ReadStringStreamIndex()
        {
            return ReadStreamIndex(StreamOffsetSizeFlags.String);
        }

        public uint ReadGuidStreamIndex()
        {
            return ReadStreamIndex(StreamOffsetSizeFlags.GUID);
        }

        public uint ReadBlobStreamIndex()
        {
            return ReadStreamIndex(StreamOffsetSizeFlags.Blob);
        }

        private uint ReadStreamIndex(StreamOffsetSizeFlags streamFlag)
        {
            return HasAFlag(streamFlag) ? ReadUInt32() : ReadUInt16();
        }
        /// <summary>
        /// Hacky, but it works
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private bool HasAFlag(StreamOffsetSizeFlags x)
        {
            if (x == StreamOffsetSizeFlags.Blob)
                return (StreamOffsetSizeFlags & StreamOffsetSizeFlags.Blob) != 0;
            else if (x == StreamOffsetSizeFlags.GUID)
                return (StreamOffsetSizeFlags & StreamOffsetSizeFlags.GUID) != 0;
            else if (x == StreamOffsetSizeFlags.String)
                return (StreamOffsetSizeFlags & StreamOffsetSizeFlags.String) != 0;
            else
                return false;
        }
    }
}
