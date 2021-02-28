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
                                OperandType = opCode.OpCodeOperandType
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
                                Operand = fi
                            });
                            i += 1;
                        }
                        break;
                    case OpCodeOperandType.ShortInlineBrTarget:
                        break;
                    case OpCodeOperandType.ShortInlineI:
                        {
                            byte fi = code[i + 1];
                            inr.Add(new ILInstruction()
                            {
                                OpCode = opCode.Value,
                                OpCodeName = opCode.Name,
                                OperandType = opCode.OpCodeOperandType,
                                Operand = fi
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
                                Operand = numb2
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
                                byte f = code[i + 4];
                                byte[] num2 = new byte[] { fi, s2, t, f };
                                short numb2 = BitConverter.ToInt16(num2, 0); //Method Token

                                //Get the method that we are calling
                                var c = mainFile.Backend.Tabels.MemberRefTabelRow[numb2 - 1]; //is the -1 needed?
                                i += 4; //skip past the string
                                #region Decode
                                //Decode the class bytes
                                DecodeMemberRefParent(c.Class, out uint tabel, out uint row);


                                var funcName = mainFile.Backend.ClrStringsStream.GetByOffset(c.Name);
                                string classs;
                                string Namespace;

                                //TYPE def
                                if (tabel == 02)
                                {
                                    var tt = mainFile.Backend.Tabels.TypeDefTabel[(int)row - 1];

                                    classs = mainFile.Backend.ClrStringsStream.GetByOffset(tt.Name);
                                    Namespace = mainFile.Backend.ClrStringsStream.GetByOffset(tt.Namespace);
                                }
                                //Type REF
                                else if (tabel == 01)
                                {
                                    var tt = mainFile.Backend.Tabels.TypeRefTabel[(int)row - 1];

                                    classs = mainFile.Backend.ClrStringsStream.GetByOffset(tt.TypeName);
                                    Namespace = mainFile.Backend.ClrStringsStream.GetByOffset(tt.TypeNamespace);
                                }
                                //Module Ref
                                else if (tabel == 26)
                                {
                                    //var tt = file.Backend.MetaDataStreamTablesHeader.Tables.ModuleRef[(int)row - 1];

                                    //classs = file.Backend.ClrStringsStream.GetByOffset(tt.Name);
                                    //Namespace = file.Backend.ClrStringsStream.GetByOffset(tt.Namespace);
                                    Console.WriteLine("Module Ref not supported!");
                                    classs = "<Module Ref>";
                                    Namespace = "<Module Ref>";
                                }
                                //Unknown
                                else
                                {
                                    classs = "<unknown>";
                                    Namespace = "<unknown>";
                                }
                                #endregion
                                var inst = new ILInstruction()
                                {
                                    OpCode = opCode.Value,
                                    OpCodeName = opCode.Name,
                                    OperandType = opCode.OpCodeOperandType
                                };

                                inst.Operand = new CallMethodDataHolder() { ClassName = classs, NameSpace = Namespace, FunctionName = funcName };
                                inr.Add(inst);
                            }
                            catch { }
                        }
                        break;
                    case OpCodeOperandType.InlineSig:
                        break;
                    case OpCodeOperandType.InlineString:
                        {
                            byte first = code[i + 1]; //1st index
                            byte sec = code[i + 2]; //2nd
                            byte third = code[i + 3];
                            byte forth = code[i + 4];
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
                                OpCodeName = opCode.Name
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
                                Operand = numb2
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
        private static void DecodeMemberRefParent(uint index, out uint tableIndex, out uint row)
        {
            tableIndex = 0;
            switch (index & MemberRefParrent)
            {
                case MemberRefParrent_TYPEDEF:
                    tableIndex = 02;
                    break;

                case MemberRefParrent_TYPEREF:
                    tableIndex = 01;
                    break;

                case MemberRefParrent_MODULEREF:
                    tableIndex = 26;
                    break;

                case MemberRefParrent_METHODDEF:
                    tableIndex = 06;
                    break;

                case MemberRefParrent_TYPESPEC:
                    tableIndex = 27;
                    break;
            }
            row = index >> 3;
        }
        #endregion
    }
}
