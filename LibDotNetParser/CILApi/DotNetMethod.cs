using LibDotNetParser.DotNet.Tabels.Defs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public bool IsImplementedByRuntime
        {
            get
            {
                return (implFlags & MethodImp.miRuntime) != 0;
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
        public bool HasReturnValue { get; }

        public MethodSignatureInfoV2 SignatureInfo { get; }

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

            //method signatures
            SignatureInfo = ParseMethodSignature(item.Signature, File, this.Name);
            this.Signature = SignatureInfo.Signature;
            this.AmountOfParms = SignatureInfo.AmountOfParms;
        }

        public static string ElementTypeToString(byte elemType)
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

        internal static MethodSignatureInfoV2 ParseMethodSignature(uint signature, DotNetFile file, string FunctionName)
        {
            MethodSignatureInfoV2 ret = new MethodSignatureInfoV2();
            string sig = "";
            var blobStreamReader = new BinaryReader(new MemoryStream(file.Backend.BlobStream));
            blobStreamReader.BaseStream.Seek(signature, SeekOrigin.Begin);
            var length = blobStreamReader.ReadByte();
            var type = blobStreamReader.ReadByte();
            var parmaters = blobStreamReader.ReadByte();
            var returnVal = ReadParam(blobStreamReader, file); //read return value

            if (type == 0)
            {
                //Static method
                sig += "static ";
                ret.IsStatic = true;
            }
            sig += returnVal.TypeInString;
            sig += " " + FunctionName;
            sig += "(";
            for (int i = 0; i < parmaters; i++)
            {
                var parm = ReadParam(blobStreamReader, file);
                ret.Params.Add(parm);
                sig += parm.TypeInString + ", ";
                ret.AmountOfParms++;
            }

            sig = sig.Substring(0, sig.Length - 2); //Remove the last ,
            sig += ");";
            ret.Signature = sig;
            return ret;
        }
        public override string ToString()
        {
            return $"{Name} in {Parrent.FullName}";
        }
        private static MethodSignatureParam ReadParam(BinaryReader r, DotNetFile file)
        {
            string sig;
            var parm = r.ReadByte();
            MethodSignatureParam ret = new MethodSignatureParam();
            switch (parm)
            {
                case 0x00:
                    {
                        //end of list
                        sig = "End of list";
                        break;
                    }
                case 0x01:
                    {
                        sig = "void";
                        ret.type = StackItemType.None;
                        break;
                    }
                case 0x02:
                    {
                        sig = "bool";
                        ret.type = StackItemType.Int32;
                        break;
                    }
                case 0x03:
                    {
                        sig = "char";
                        ret.type = StackItemType.Int32;
                        break;
                    }
                case 0x04:
                    {
                        sig = "sbyte";
                        ret.type = StackItemType.Int32;
                        break;
                    }
                case 0x05:
                    {
                        sig = "byte";
                        ret.type = StackItemType.Int32;
                        break;
                    }
                case 0x06:
                    {
                        sig = "short";
                        ret.type = StackItemType.Int32;
                        break;
                    }
                case 0x07:
                    {
                        sig = "ushort";
                        ret.type = StackItemType.Int32;
                        break;
                    }
                case 0x08:
                    {
                        sig = "int";
                        ret.type = StackItemType.Int32;
                        break;
                    }
                case 0x09:
                    {
                        sig = "uint";
                        ret.type = StackItemType.Int32;
                        break;
                    }
                case 0x0A:
                    {
                        sig = "long";
                        ret.type = StackItemType.Int64;
                        break;
                    }
                case 0x0B:
                    {
                        sig = "ulong";
                        ret.type = StackItemType.Int64;
                        break;
                    }
                case 0x0C:
                    {
                        sig = "float";
                        ret.type = StackItemType.Float32;
                        break;
                    }
                case 0x0D:
                    {
                        sig = "double";
                        ret.type = StackItemType.Float64;
                        break;
                    }
                case 0x0E:
                    {
                        sig = "string";
                        ret.type = StackItemType.String;
                        break;
                    }
                case 0xF:
                    //Pointer* (followed by type
                    throw new System.NotImplementedException();
                case 0x10:
                    //byref* //followed by type
                    throw new System.NotImplementedException();
                case 0x11:
                    //valve type (followed by typedef or typeref token)
                    {
                        var t = r.ReadByte(); //type of the type
                        IlDecompiler.DecodeTypeDefOrRef(t, out uint rowType, out uint index);
                        string name;
                        string Namespace;
                        ret.type = StackItemType.Object;
                        ret.IsClass = true;
                        if (rowType == 0)
                        {
                            //typedef
                            var tt = file.Backend.Tabels.TypeDefTabel[(int)(index - 1)];
                            name = file.Backend.ClrStringsStream.GetByOffset(tt.Name);
                            Namespace = file.Backend.ClrStringsStream.GetByOffset(tt.Namespace);

                            //resolve it
                            foreach (var item in file.Types)
                            {
                                if (item.NameSpace == Namespace && item.Name == name)
                                {
                                    ret.ClassType = item;
                                }
                            }
                        }
                        else if (rowType == 1)
                        {
                            //typeref
                            var tt = file.Backend.Tabels.TypeRefTabel[(int)(index - 1)];
                            name = file.Backend.ClrStringsStream.GetByOffset(tt.TypeName);
                            Namespace = file.Backend.ClrStringsStream.GetByOffset(tt.TypeNamespace);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        if (!string.IsNullOrEmpty(Namespace))
                            sig = $"{Namespace}.{name}";
                        else
                            sig = $"{name}";
                        break;
                    }
                case 0x12:
                    //class followed by typedef or typeref token
                    {
                        var t = r.ReadByte(); //type of the type
                        IlDecompiler.DecodeTypeDefOrRef(t, out uint rowType, out uint index);
                        string name;
                        string Namespace;
                        ret.type = StackItemType.Object;
                        ret.IsClass = true;
                        if (rowType == 0)
                        {
                            //typedef
                            var tt = file.Backend.Tabels.TypeDefTabel[(int)(index - 1)];
                            name = file.Backend.ClrStringsStream.GetByOffset(tt.Name);
                            Namespace = file.Backend.ClrStringsStream.GetByOffset(tt.Namespace);

                            //resolve it
                            foreach (var item in file.Types)
                            {
                                if (item.NameSpace == Namespace && item.Name == name)
                                {
                                    ret.ClassType = item;
                                }
                            }
                        }
                        else if (rowType == 1)
                        {
                            //typeref
                            var tt = file.Backend.Tabels.TypeRefTabel[(int)(index - 1)];
                            name = file.Backend.ClrStringsStream.GetByOffset(tt.TypeName);
                            Namespace = file.Backend.ClrStringsStream.GetByOffset(tt.TypeNamespace);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        if (!string.IsNullOrEmpty(Namespace))
                            sig = $"{Namespace}.{name}";
                        else
                            sig = $"{name}";
                        break;
                    }
                case 0x13:
                    //GENERIC_PARM
                    {
                        var b = r.ReadByte();
                        ;
                        sig = "todo";
                        break;
                    }
                case 0x14:
                    {
                        sig = "[][]";
                        ret.type = StackItemType.Array;
                        break;
                    }
                case 0x15:
                    {
                        //ELEMENT_TYPE_GENERICINST
                        var t = r.ReadByte(); //type of the generic type
                        var c = r.ReadByte(); //generic type (TypeDefOrRefEncoded)
                        var d = r.ReadByte(); //Count of generic args
                        IlDecompiler.DecodeTypeDefOrRef(c, out uint rowType, out uint index);
                        ret.IsGeneric = true;
                        ret.type = StackItemType.Object;
                        if (rowType == 0)
                        {
                            //typedef
                            var tt = file.Backend.Tabels.TypeDefTabel[(int)(index - 1)];
                            var name = file.Backend.ClrStringsStream.GetByOffset(tt.Name);
                            var Namespace = file.Backend.ClrStringsStream.GetByOffset(tt.Namespace);

                            if (!string.IsNullOrEmpty(Namespace))
                                sig = $"{Namespace}.{name}<";
                            else
                                sig = $"{name}<";

                            ret.GenericClassNamespace = Namespace;
                            ret.GenericClassName = name;
                        }
                        else if (rowType == 1)
                        {
                            //typeref
                            var tt = file.Backend.Tabels.TypeRefTabel[(int)(index - 1)];
                            var name = file.Backend.ClrStringsStream.GetByOffset(tt.TypeName);
                            var Namespace = file.Backend.ClrStringsStream.GetByOffset(tt.TypeNamespace);

                            if (!string.IsNullOrEmpty(Namespace))
                                sig = $"{Namespace}.{name}<";
                            else
                                sig = $"{name}<";

                            ret.GenericClassNamespace = Namespace;
                            ret.GenericClassName = name;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    }
                case 0x16:
                    {
                        //TypeRef
                        throw new System.NotImplementedException();
                    }
                case 0x18:
                    {
                        //IntPtr
                        sig = "IntPtr";
                        ret.type = StackItemType.Int32;
                        break;
                    }
                case 0x19:
                    {
                        //UIntPtr
                        sig = "UIntPtr";
                        ret.type = StackItemType.Int32;
                        break;
                    }
                case 0x1B:
                    {
                        sig = "func ptr";
                        ret.type = StackItemType.MethodPtr;
                        break;
                    }
                case 0x1C:
                    {
                        sig = "object";
                        ret.type = StackItemType.Object;
                        break;
                    }
                case 0x1D:
                    {
                        sig = "[]";
                        ret.type = StackItemType.Array;
                        break;
                    }
                case 0x1E:
                    //MVar
                    throw new System.NotImplementedException();
                default:
                    throw new System.NotImplementedException("Unknown byte: 0x" + parm.ToString("X"));
            }

            ret.TypeInString = sig;
            return ret;
        }
        public class MethodSignatureInfoV2
        {
            public MethodSignatureParam ReturnVal { get; set; }
            public List<MethodSignatureParam> Params = new List<MethodSignatureParam>();
            public bool IsStatic { get; set; } = false;
            public string Signature { get; set; } = "";
            public int AmountOfParms { get; set; } = 0;
        }
        public class MethodSignatureParam
        {
            public StackItemType type;
            public string TypeInString;

            public bool IsGeneric { get; set; } = false;
            public string GenericClassNamespace { get; set; }
            public string GenericClassName { get; set; }
            public bool IsClass { get; set; } = false;
            public DotNetType ClassType { get; set; }
        }
    }
}