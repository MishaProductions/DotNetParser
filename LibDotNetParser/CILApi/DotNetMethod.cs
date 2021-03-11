using LibDotNetParser;
using LibDotNetParser.DotNet.Tabels.Defs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDotNetParser.CILApi
{
    public class DotNetMethod
    {
        private PEFile file;
        private DotNetFile file2;
        private Method method;

        MethodAttr flags;

        public string Name { get; private set; }
        public uint RVA { get { return method.RVA; } }
        public uint Offset
        {
            get
            {
                return (uint)BinUtil.RVAToOffset(RVA, file.PeHeader.Sections);
            }
        }
        public DotNetFile File
        {
            get
            {
                return file2;
            }
        }
        public bool IsStatic
        {
            get
            {
                return (flags & MethodAttr.mdStatic) != 0;
            }
        }

        public bool IsExtern
        {
            get
            {
                return (flags & MethodAttr.mdUnmanagedExport) != 0;
            }
        }


        public string Signature { get; set; }
        public DotNetType Parrent { get; }
        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="file"></param>
        /// <param name="item"></param>
        /// <param name="parrent"></param>
        public DotNetMethod(PEFile file, Method item, DotNetType parrent)
        {
            this.file = file;
            this.method = item;
            this.Parrent = parrent;
            this.flags = (MethodAttr)item.Flags;
            this.file2 = parrent.File;

            this.Signature = file.ClrStringsStream.GetByOffset(item.Signature);
            this.Name = file.ClrStringsStream.GetByOffset(item.Name);
        }
        /// <summary>
        /// Gets the raw IL instructions for this method.
        /// </summary>
        /// <returns>raw IL instructions</returns>
        public byte[] GetBody()
        {
            var fs = file.RawFile;
            fs.BaseStream.Seek(Offset, System.IO.SeekOrigin.Begin);

            byte format = fs.ReadByte();
            int CodeSize = 0;
            var verytinyheader = format.ConvertByteToBoolArray();


            var header = BinUtil.ConvertBoolArrayToByte(new bool[] { verytinyheader[6], verytinyheader[7] });

            var sizer = BinUtil.ConvertBoolArrayToByte(new bool[] { verytinyheader[0], verytinyheader[1], verytinyheader[2], verytinyheader[3], verytinyheader[4], verytinyheader[5], });
            fs.BaseStream.Seek(Offset + 1, System.IO.SeekOrigin.Begin);
            byte form2 = fs.ReadByte();

            fs.BaseStream.Seek(Offset + 1, System.IO.SeekOrigin.Begin);

            if (header == 3) //Fat format
            {
                byte info2 = fs.ReadByte(); //some info on header
                ushort MaxStack = fs.ReadUInt16();
                CodeSize = (int)fs.ReadUInt32();
                uint LocalVarSigTok = fs.ReadUInt32();
            }
            else //Tiny format
            {
                CodeSize = sizer;
            }
            List<byte> code = new List<byte>();

            for (uint i = Offset + 1; i < Offset + 1 + CodeSize; i++)
            {
                byte opcode = fs.ReadByte();

                code.Add(opcode);
            }

            return code.ToArray();
        }
    }
}
