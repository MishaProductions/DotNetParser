using LibDotNetParser.DotNet.Tabels.Defs;
using System.Collections.Generic;
using System.IO;

namespace LibDotNetParser.CILApi
{
    public class DotNetMethod
    {
        private readonly PEFile file;
        private readonly DotNetFile file2;
        private readonly MethodAttr flags;
        private readonly MethodImp implFlags;

        public Method BackendTabel;

        /// <summary>
        /// Function Signature.
        /// </summary>
        public string Signature
        {
            get;
            private set;
        }

        public List<MethodArgStack> Parms = new List<MethodArgStack>();
        public string Name { get; private set; }
        public uint RVA { get { return BackendTabel.RVA; } }
        public uint Offset
        {
            get
            {
                return (uint)BinUtil.RVAToOffset(RVA, file.PeHeader.Sections);
            }
        }
        public int AmountOfParms { get; private set; }
        public StackItemType StartParm { get; private set; }
        public StackItemType EndParm { get; private set; }
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

        public bool IsInternalCall
        {
            get
            {
                return (implFlags & MethodImp.miInternalCall) != 0;
            }
        }

        public bool IsExtern
        {
            get
            {
                return RVA == 0;
            }
        }
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
            this.BackendTabel = item;
            this.Parrent = parrent;
            this.flags = (MethodAttr)item.Flags;
            this.implFlags = (MethodImp)item.ImplFlags;
            this.file2 = parrent.File;

            this.Name = file.ClrStringsStream.GetByOffset(item.Name);
            var sig2 = ParseMethodSignature(item.Signature, File, this.Name);
            this.Signature = sig2.Signature;
            this.AmountOfParms = sig2.AmountOfParms;
            this.StartParm = sig2.FirstParm;
            this.EndParm = sig2.LastParm;
            string sig = "";

            var blobStreamReader = new BinaryReader(new MemoryStream(file.BlobStream));
            blobStreamReader.BaseStream.Seek(item.Signature, SeekOrigin.Begin);
            var length = blobStreamReader.ReadByte();
            var type = blobStreamReader.ReadByte();
            var parmaters = blobStreamReader.ReadByte();
            var returnType = blobStreamReader.ReadByte();
            if (type == 0)
            {
                //Static method
                sig += "static ";
            }

            for (int i = 0; i < parmaters; i++)
            {
                var parm = blobStreamReader.ReadByte();
                if (parm == 0x0E)
                {
                    Parms.Add(new MethodArgStack() { type = StackItemType.String});
                }
            }

        }

        private static string ElementTypeToString(byte elemType)
        {
            switch (elemType)
            {
                case 0x00:
                    return "END";
                case 0x01:
                    return "void";
                case 0x02:
                    return "bool";
                case 0x03:
                    return "char";
                case 0x04:
                    return "sbyte";
                case 0x05:
                    return "byte";
                case 0x06:
                    return "short";
                case 0x07:
                    return "ushort";
                case 0x08:
                    return "int";
                case 0x09:
                    return "uint";
                case 0x0A:
                    return "long";
                case 0x0B:
                    return "ulong";
                case 0x0C:
                    return "float";
                case 0x0D:
                    return "double";
                case 0x0E:
                    return "string";
                case 0xF:
                    return "pointer*"; //followed by type
                case 0x10:
                    return "byref*"; //followed by type
                case 0x11:
                    return "valveType"; //followed by TypeDef or TypeRef token
                case 0x12:
                    return "CLASS"; //followed by typedef or typeref token
                case 0x13:
                    return "GENERIC_PARM";
                case 0x14:
                    return "Array";
                case 0x15:
                    return "ELEMENT_TYPE_GENERICINST";
                case 0x16:
                    return "TypeRef";
                case 0x18:
                    return "IntPtr";
                case 0x19:
                    return "UIntPtr";
                case 0x1B:
                    return "function pointer";
                case 0x1C:
                    return "object";
                case 0x1D:
                    return "[]";
                case 0x1E:
                    return "mvar";
                default:
                    return "<Unknown>";
            }
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

        internal static MethodSignatureInfo ParseMethodSignature(uint signature, DotNetFile file, string FunctionName)
        {
            string sig=""; 
             var blobStreamReader = new BinaryReader(new MemoryStream(file.Backend.BlobStream));
            blobStreamReader.BaseStream.Seek(signature, SeekOrigin.Begin);
            var length = blobStreamReader.ReadByte();
            var type = blobStreamReader.ReadByte();
            var parmaters = blobStreamReader.ReadByte();
            var returnType = blobStreamReader.ReadByte();
            if (type == 0)
            {
                //Static method
                sig += "static ";
            }
            MethodSignatureInfo info = new MethodSignatureInfo();
            sig += ElementTypeToString(returnType);
            sig += " " + FunctionName;
            sig += "(";
            for (int i = 0; i < parmaters; i++)
            {
                var parm = blobStreamReader.ReadByte();
                sig += ElementTypeToString(parm) + ",";
                if (i == 0)
                {
                    if (ElementTypeToString(parm) == "string")
                    {
                        info.FirstParm = StackItemType.String;
                    }
                    else if (ElementTypeToString(parm) == "int")
                    {
                        info.FirstParm = StackItemType.Int32;
                    }
                    else if (ElementTypeToString(parm) == "long")
                    {
                        info.FirstParm = StackItemType.Int64;
                    }
                }
                else if (i == parmaters)
                {
                    if (ElementTypeToString(parm) == "string")
                    {
                        info.LastParm = StackItemType.String;
                    }
                    else if (ElementTypeToString(parm) == "int")
                    {
                        info.LastParm = StackItemType.Int32;
                    }
                    else if (ElementTypeToString(parm) == "long")
                    {
                        info.LastParm = StackItemType.Int64;
                    }
                }
                info.AmountOfParms++;
            }
            sig = sig.TrimEnd(','); //Remove the last ,
            sig += ");";
            info.Signature = sig;
            return info;
        }
    }
    class MethodSignatureInfo { public int AmountOfParms; public string Signature; public StackItemType FirstParm; public StackItemType LastParm; }
}
