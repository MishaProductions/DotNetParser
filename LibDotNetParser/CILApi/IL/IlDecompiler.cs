using LibDotNetParser.CILApi.IL;
using System;
using System.Collections.Generic;

namespace LibDotNetParser.CILApi
{
    public class IlDecompiler
    {
        private DotNetMethod m;
        private DotNetFile mainFile;
        public IlDecompiler(DotNetMethod method)
        {
            m = method;
            mainFile = m.File;
        }

        public ILInstruction[] Decompile()
        {
            List<ILInstruction> inr = new List<ILInstruction>();
            byte[] code = m.GetBody();

            for (int i = 0; i < code.Length; i++)
            {
                byte opCodeb = code[i];
                var opCode = OpCodes.SingleOpCodes[opCodeb];
                if (opCode == null)
                    continue;

                if (opCodeb == 0xFE)
                {
                    opCodeb = code[i + 1];
                    opCode = OpCodes.MultiOpCodes[opCodeb];
                    i++;
                }

                //TODO: Implment the rest of these
                switch (opCode.OpCodeOperandType)
                {
                    // other
                    case OpCodeOperandType.InlineNone:
                        {
                            inr.Add(new ILInstruction()
                            {
                                OpCode = opCode.Value,
                                OpCodeName = opCode.Name,
                                OperandType = opCode.OpCodeOperandType,
                                Position = i
                            });
                        }
                        break;
                    case OpCodeOperandType.InlinePhi:
                        //Never should be used
                        break;
                    case OpCodeOperandType.InlineTok:
                        break;
                    //8 bit int operand
                    case OpCodeOperandType.ShortInlineVar:
                        {
                            byte fi = code[i + 1];
                            inr.Add(new ILInstruction()
                            {
                                OpCode = opCode.Value,
                                OpCodeName = opCode.Name,
                                OperandType = opCode.OpCodeOperandType,
                                Operand = fi,
                                Position = i
                            });
                            i += 1;
                        }
                        break;
                    case OpCodeOperandType.ShortInlineBrTarget:
                        {
                            throw new NotImplementedException();
                            byte fi = code[i + 1];
                            inr.Add(new ILInstruction()
                            {
                                OpCode = opCode.Value,
                                OpCodeName = opCode.Name,
                                OperandType = opCode.OpCodeOperandType,
                                Operand = fi,
                                Position = i
                            });
                            i += 1;
                        }
                        break;
                    case OpCodeOperandType.ShortInlineI:
                        {
                            byte fi = code[i + 1];
                            inr.Add(new ILInstruction()
                            {
                                OpCode = opCode.Value,
                                OpCodeName = opCode.Name,
                                OperandType = opCode.OpCodeOperandType,
                                Operand = fi,
                                Position = i
                            });
                            i += 1;
                        }
                        break;
                    // 16 bit int
                    case OpCodeOperandType.InlineVar:
                        break;
                    // 32 bit int
                    case OpCodeOperandType.InlineI:
                        {
                            byte fi = code[i + 1];
                            byte s2 = code[i + 2];
                            byte t = code[i + 3];
                            byte f = code[i + 4];
                            byte[] num2 = new byte[] { fi, s2, t, f };
                            var numb2 = BitConverter.ToInt32(num2, 0);
                            inr.Add(new ILInstruction()
                            {
                                OpCode = opCode.Value,
                                OpCodeName = opCode.Name,
                                OperandType = opCode.OpCodeOperandType,
                                Operand = numb2,
                                Position = i
                            });
                            i += 3;
                        }
                        break;
                    case OpCodeOperandType.InlineBrTarget:
                        break;
                    case OpCodeOperandType.InlineField:
                        break;
                    case OpCodeOperandType.InlineMethod:
                        {
                            try
                            {
                                byte fi = code[i + 1];
                                byte s2 = code[i + 2];
                                byte t = code[i + 3];
                                byte f = code[i + 4]; //Method type. 6=Method,10=MemberRef
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

                                    var inst = new ILInstruction()
                                    {
                                        OpCode = opCode.Value,
                                        OpCodeName = opCode.Name,
                                        OperandType = opCode.OpCodeOperandType,
                                        Position = i
                                    };


                                    inst.Operand = new InlineMethodOperandData()
                                    {
                                        NameSpace = mainFile.Backend.ClrStringsStream.GetByOffset(Namespace),
                                        ClassName = mainFile.Backend.ClrStringsStream.GetByOffset(classs),
                                        FunctionName = funcName,
                                        RVA = 0
                                    };
                                    inr.Add(inst);

                                    i += 4; //skip past the string
                                    continue;
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
                                        Position = i
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

                                    inst.Operand = new InlineMethodOperandData()
                                    {
                                        NameSpace = Namespace,
                                        ClassName = className,
                                        FunctionName = name,
                                        RVA = c.RVA
                                    };

                                    inr.Add(inst);

                                    i += 4; //skip past the string
                                    continue;
                                }

                            }
                            catch { }
                        }
                        break;
                    case OpCodeOperandType.InlineSig:
                        break;
                    case OpCodeOperandType.InlineString:
                        {
                            byte first = code[i + 1]; //1st index
                            byte sec = code[i + 2];   //2nd
                            byte third = code[i + 3]; //3rd
                            byte forth = code[i + 4]; //string type
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
                            i += 4; //skip past the string

                            inr.Add(new ILInstruction()
                            {
                                OpCode = opCode.Value,
                                Operand = s,
                                OpCodeName = opCode.Name,
                                Position = i-4
                            });
                        }
                        break;
                    case OpCodeOperandType.InlineSwitch:
                        break;
                    case OpCodeOperandType.ShortInlineR:
                        break;
                    case OpCodeOperandType.InlineType:
                        break;
                    // 64 bit int
                    case OpCodeOperandType.InlineI8:
                        {
                            byte fi = code[i + 1];
                            byte s2 = code[i + 2];
                            byte t = code[i + 3];
                            byte f = code[i + 4];
                            byte a = code[i + 5];
                            byte b = code[i + 6];
                            byte c = code[i + 7];
                            byte d = code[i + 8];

                            byte[] num2 = new byte[] { fi, s2, t, f, a, b, c, d };
                            var numb2 = BitConverter.ToInt64(num2, 0);
                            inr.Add(new ILInstruction()
                            {
                                OpCode = opCode.Value,
                                OpCodeName = opCode.Name,
                                OperandType = opCode.OpCodeOperandType,
                                Operand = numb2,
                                Position = i
                            });
                            i += 7;
                        }
                        break;
                    case OpCodeOperandType.InlineR:
                        break;



                    default:
                        break;
                }
            }

            return inr.ToArray();
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
