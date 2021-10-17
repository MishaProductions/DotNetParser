using LibDotNetParser.CILApi.IL;
using System;
using System.Collections.Generic;

namespace LibDotNetParser.CILApi
{
    public class IlDecompiler
    {
        private DotNetMethod m;
        private DotNetFile mainFile;
        private byte[] code;
        private List<DotNetFile> ContextTypes = new List<DotNetFile>();

        public IlDecompiler(DotNetMethod method)
        {
            if (method == null)
                throw new ArgumentException("method");

            m = method;
            mainFile = m.File;
            code = m.GetBody();
            AddRefernce(m.Parrent.File);
        }
        public void AddRefernce(DotNetFile f)
        {
            ContextTypes.Add(f);
        }

        public ILInstruction[] Decompile()
        {
            List<ILInstruction> inr = new List<ILInstruction>();
            int Instructions = 0;
            for (int i = 0; i < code.Length; i++)
            {
                var instruction = GetInstructionAtOffset(i, Instructions);
                if (instruction != null)
                {
                    inr.Add(instruction);


                    i += instruction.Size;
                    Instructions++;
                }

            }

            return inr.ToArray();
        }

        public ILInstruction GetInstructionAtOffset(int Offset, int relPostion)
        {
            byte opCodeb = code[Offset];
            var opCode = OpCodes.SingleOpCodes[opCodeb];

            int size = 0;
            if (opCodeb == 0xFE)
            {
                opCodeb = code[Offset + 1];
                opCode = OpCodes.MultiOpCodes[opCodeb];
                Offset++;
                size++;
            }

            ILInstruction ret = new ILInstruction() { OpCode = opCode.Value, OpCodeName = opCode.Name, OperandType = opCode.OpCodeOperandType, Position = Offset, RelPosition = relPostion, Size = size };

            if (relPostion == -1)
            {
                var arr = Decompile();
                foreach (var item in arr)
                {
                    if (item.Position == Offset)
                    {
                        return item;
                    }
                }
            }
            //TODO: Implment the rest of these
            switch (opCode.OpCodeOperandType)
            {
                case OpCodeOperandType.InlineNone:
                    {
                        return ret;
                    }
                case OpCodeOperandType.InlinePhi:
                    //Never should be used
                    throw new InvalidOperationException();
                case OpCodeOperandType.InlineTok:
                    throw new InvalidOperationException();
                //8 bit int operand
                case OpCodeOperandType.ShortInlineVar:
                    {
                        byte fi = code[Offset + 1];
                        ret.Size = +1;
                        ret.Operand = fi;
                        return ret;
                    }
                case OpCodeOperandType.ShortInlineBrTarget:
                    {
                        sbyte fi = (sbyte)code[Offset + 1];
                        ret.Size = +1;
                        ret.Operand = fi + 1;
                        return ret;
                    }
                case OpCodeOperandType.ShortInlineI:
                    {
                        byte fi = code[Offset + 1];
                        ret.Size = +1;
                        ret.Operand = fi;
                        return ret;
                    }
                // 16 bit int
                case OpCodeOperandType.InlineVar:
                    throw new NotImplementedException();
                // 32 bit int
                case OpCodeOperandType.InlineI:
                    {
                        byte fi = code[Offset + 1];
                        byte s2 = code[Offset + 2];
                        byte t = code[Offset + 3];
                        byte f = code[Offset + 4];
                        byte[] num2 = new byte[] { fi, s2, t, f };
                        var numb2 = BitConverter.ToInt32(num2, 0);

                        ret.Size += 4;
                        ret.Operand = numb2;
                        return ret;
                    }
                case OpCodeOperandType.InlineBrTarget:
                    throw new NotImplementedException();
                case OpCodeOperandType.InlineField:
                    {
                        byte fi = code[Offset + 1];
                        byte s2 = code[Offset + 2];
                        byte t = code[Offset + 3];
                        byte f = code[Offset + 4];
                        byte[] num2 = new byte[] { fi, s2, t, f };
                        var numb2 = BitConverter.ToInt32(num2, 0);

                        ret.Size += 4;
                        ret.Operand = fi;
                        return ret;
                    }
                case OpCodeOperandType.InlineMethod:
                    {
                        try
                        {
                            byte fi = code[Offset + 1];
                            byte s2 = code[Offset + 2];
                            byte t = code[Offset + 3];
                            byte f = code[Offset + 4]; //Method type. 6=Method,10=MemberRef
                            byte[] num2 = new byte[] { fi, s2, t, 0 };
                            var numb2 = BitConverter.ToInt32(num2, 0); //Method Token


                            if (f == 10) //MemberRef
                            {
                                //Get the method that we are calling
                                var c = mainFile.Backend.Tabels.MemberRefTabelRow[numb2 - 1];

                                #region Decode
                                //Decode the class bytes
                                DecodeMemberRefParent(c.Class, out MemberRefParentType tabel, out uint row);


                                var funcName = mainFile.Backend.ClrStringsStream.GetByOffset(c.Name);
                                uint classs;
                                uint Namespace;

                                //TYPE def
                                if (tabel == MemberRefParentType.TypeDef)
                                {
                                    var tt = mainFile.Backend.Tabels.TypeDefTabel[(int)row - 1];

                                    classs = tt.Name;
                                    Namespace = tt.Namespace;
                                    
                                }
                                //Type REF
                                else if (tabel == MemberRefParentType.TypeRef)
                                {
                                    var tt = mainFile.Backend.Tabels.TypeRefTabel[(int)row - 1];

                                    classs = tt.TypeName;

                                    
                                    Namespace = tt.TypeNamespace;
                                }
                                //Module Ref
                                else if (tabel == MemberRefParentType.ModuleRef)
                                {
                                    var tt = mainFile.Backend.Tabels.ModuleRefTabel[(int)row - 1];

                                    classs = tt.Name;
                                    Namespace = tt.Name;
                                }
                                //Unknown
                                else
                                {
                                    classs = 0;
                                    Namespace = 0;
                                    throw new NotImplementedException();
                                }
                                #endregion

                                ret.Size += 4;
                                var anamespace=mainFile.Backend.ClrStringsStream.GetByOffset(Namespace);
                                var typeName = mainFile.Backend.ClrStringsStream.GetByOffset(classs);

                                //Now, resolve this method
                                //TODO: Resolve the method properly by first
                                //1) resolve the type of the method
                                //2) search for the method in the type
                                //3) get method RVA
                                
                                //For now, resolve it by name

                                DotNetMethod m = null;
                                foreach (var type in ContextTypes)
                                {
                                    foreach (var item in type.Types)
                                    {
                                        foreach (var meth in item.Methods)
                                        {
                                            if (meth.Name == funcName && meth.Parrent.Name == typeName && meth.Parrent.NameSpace == anamespace)
                                            {
                                                m = meth;
                                            }
                                        }
                                    }

                                }
                                uint rva = 0;
                                if (m != null)
                                    rva = m.RVA;
                                else
                                    Console.WriteLine($"[ILDecompiler: WARN] Cannot resolve method RVA. Are you missing a call to AddRefernce()??. Method data: {anamespace}.{typeName}.{funcName}");


                                ret.Operand = new InlineMethodOperandData()
                                {
                                    NameSpace = anamespace,
                                    ClassName = typeName,
                                    FunctionName = funcName,
                                    RVA = rva,
                                    Signature = DotNetMethod.ParseMethodSignature(c.Signature, mainFile, funcName).Signature
                                };
                                return ret;
                            }
                            else if (f == 6)//method
                            {
                                //Get the method that we are calling
                                var c = mainFile.Backend.Tabels.MethodTabel[numb2 - 1];




                                var inst = new ILInstruction()
                                {
                                    OpCode = opCode.Value,
                                    OpCodeName = opCode.Name,
                                    OperandType = opCode.OpCodeOperandType,
                                    Position = Offset,
                                    Size = 4
                                };
                                string name = mainFile.Backend.ClrStringsStream.GetByOffset(c.Name);
                                //Now, resolve this method
                                DotNetMethod m = null;
                                foreach (var item in mainFile.Types)
                                {
                                    foreach (var meth in item.Methods)
                                    {
                                        if (meth.RVA == c.RVA && meth.Name == name)
                                        {
                                            m = meth;
                                        }
                                    }
                                }

                                string className = "CannotFind";
                                string Namespace = "CannotFind";

                                if (m != null)
                                {
                                    className = m.Parrent.Name;
                                    Namespace = m.Parrent.NameSpace;
                                }
                                ret.Size += 4;
                                ret.Operand = new InlineMethodOperandData()
                                {
                                    NameSpace = Namespace,
                                    ClassName = className,
                                    FunctionName = name,
                                    RVA = c.RVA,
                                    Signature = m.Signature,

                                };
                                return ret;
                            }

                        }
                        catch { }
                    }
                    break;
                case OpCodeOperandType.InlineSig:
                    throw new NotImplementedException();
                case OpCodeOperandType.InlineString:
                    {
                        byte first = code[Offset + 1]; //1st index
                        byte sec = code[Offset + 2];   //2nd
                        byte third = code[Offset + 3]; //3rd
                        byte forth = code[Offset + 4]; //string type
                        byte[] num = new byte[] { first, sec, third, 0 };
                        var numb = BitConverter.ToInt32(num, 0);

                        //Get the string
                        string s;

                        if (forth != 112)
                        {
                            //Will this ever be in the String Stream?
                            s = mainFile.Backend.ClrStringsStream.GetByOffset((uint)numb);
                        }
                        else
                        {
                            //US stream
                            s = mainFile.Backend.ClrUsStream.GetByOffset((uint)numb);
                        }
                        ret.Size += 4;
                        ret.Operand = s;
                        return ret;
                    }
                case OpCodeOperandType.InlineSwitch:
                    throw new NotImplementedException();
                case OpCodeOperandType.ShortInlineR:
                    throw new NotImplementedException();
                case OpCodeOperandType.InlineType:
                    {
                        byte fi = code[Offset + 1];
                        byte s2 = code[Offset + 2];
                        byte t = code[Offset + 3];
                        byte f = code[Offset + 4];
                        byte[] num2 = new byte[] { fi, s2, t, f };
                        var numb2 = BitConverter.ToInt32(num2, 0);

                        ret.Size += 4;
                        ret.Operand = fi;
                        return ret;
                    }
                // 64 bit int
                case OpCodeOperandType.InlineI8:
                    {
                        byte fi = code[Offset + 1];
                        byte s2 = code[Offset + 2];
                        byte t = code[Offset + 3];
                        byte f = code[Offset + 4];
                        byte a = code[Offset + 5];
                        byte b = code[Offset + 6];
                        byte c = code[Offset + 7];
                        byte d = code[Offset + 8];

                        byte[] num2 = new byte[] { fi, s2, t, f, a, b, c, d };
                        var numb2 = BitConverter.ToInt64(num2, 0);
                        ret.Size += 8;
                        ret.Operand = numb2;
                        return ret;
                    }
                case OpCodeOperandType.InlineR:
                    throw new NotImplementedException();
                default:
                    break;
            }

            return null;
        }
        #region Decoding MemberRefParent
        private const uint MemberRefParrent = 0x7;
        private const uint MemberRefParrent_TYPEDEF = 0x0;
        private const uint MemberRefParrent_TYPEREF = 0x1;
        private const uint MemberRefParrent_MODULEREF = 0x2;
        private const uint MemberRefParrent_METHODDEF = 0x3;
        private const uint MemberRefParrent_TYPESPEC = 0x4;
        private static void DecodeMemberRefParent(uint index, out MemberRefParentType tableIndex, out uint row)
        {
            tableIndex = 0;
            switch (index & MemberRefParrent)
            {
                case MemberRefParrent_TYPEDEF:
                    tableIndex = MemberRefParentType.TypeDef;
                    break;

                case MemberRefParrent_TYPEREF:
                    tableIndex = MemberRefParentType.TypeRef;
                    break;

                case MemberRefParrent_MODULEREF:
                    tableIndex = MemberRefParentType.ModuleRef;
                    break;

                case MemberRefParrent_METHODDEF:
                    tableIndex = MemberRefParentType.MethodDef;
                    break;

                case MemberRefParrent_TYPESPEC:
                    tableIndex = MemberRefParentType.TypeSpec;
                    break;
            }
            row = index >> 3;
        }

        public enum MemberRefParentType
        {
            TypeDef,
            TypeRef,
            ModuleRef,
            MethodDef,
            TypeSpec
        }
        #endregion
    }
}
