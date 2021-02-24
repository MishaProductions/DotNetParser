using LibDotNetParser;
using LibDotNetParser.DotNet.Tabels.Defs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDotNetParser.CILApi
{
    public class DotNetMethod
    {
        private PEParaser file;
        private DotNetFile file2;
        private MethodTabelRow method;

        MethodAttr flags;

        public string Name { get; private set; }
        public uint RVA { get { return method.RVA; } }
        public uint Offset
        {
            get
            {
                return (uint)PEParaser.RelativeVirtualAddressToFileOffset(RVA, file.PeHeader.Sections);
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
        public string Signature { get; set; }
        public DotNetType Parrent { get; }
        uint nextMethod;
        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="file"></param>
        /// <param name="item"></param>
        /// <param name="parrent"></param>
        public DotNetMethod(PEParaser file, MethodTabelRow item, DotNetType parrent, uint nextMethod)
        {
            this.file = file;
            this.method = item;
            this.Parrent = parrent;
            this.flags = (MethodAttr)item.Flags;
            this.nextMethod = nextMethod;
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
            Console.WriteLine(fs == null);
            if (format == 27 | format == 19)
            {
                //Fat format
                byte info2 = fs.ReadByte(); //some info on header
                ushort MaxStack = fs.ReadUInt16();
                CodeSize = (int)fs.ReadUInt32();
                uint LocalVarSigTok = fs.ReadUInt32();
            }
            else
            {
                //Tiny format (2nd byte is code size)
                var size = format.ToString()[1];
                CodeSize = int.Parse(size.ToString());
            }
            List<byte> code = new List<byte>();

            if (file.tabels.MethodTabel.Count <= nextMethod)
            {
                for (uint i = Offset + 1; i < (Offset + CodeSize); i++)
                {
                    //fs.BaseStream.Seek(i, System.IO.SeekOrigin.Begin);
                    byte opcode = fs.ReadByte();

                    code.Add(opcode);
                }
            }
            else
            {
                uint offset = (uint)PEParaser.RelativeVirtualAddressToFileOffset(file.tabels.MethodTabel[(int)nextMethod].RVA, file.PeHeader.Sections);
                for (uint i = Offset + 1; i < offset; i++)
                {
                    //fs.BaseStream.Seek(i, System.IO.SeekOrigin.Begin);
                    byte opcode = fs.ReadByte();

                    code.Add(opcode);
                }
            }

            return code.ToArray();
        }
    }
}
