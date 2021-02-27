using LibDotNetParser.CILApi.IL;
using System;
using System.Collections.Generic;
using System.Text;

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
                byte opCode = code[i];
                if (opCode == OpCodes.Ldstr)
                {
                    //Decode the number
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

                        //This is only a temp. hack
                        s = mainFile.Backend.ClrUsStream.GetByOffset((uint)numb);
                    }
                    i += 4; //skip past the string

                    inr.Add(new ILInstruction()
                    {
                        OpCode = OpCodes.Ldstr,
                        Operand = s,
                        OpCodeName = "ldstr"
                    });
                }
                else if (opCode == OpCodes.Call)
                {
                    try
                    {
                        byte fi = code[i + 1];
                        byte s = code[i + 2];
                        byte t = code[i + 3];
                        byte f = code[i + 4];
                        byte[] num = new byte[] { fi, s, t, f };
                        short numb = BitConverter.ToInt16(num, 0); //Method Token

                        //Get the method that we are calling
                        var c = mainFile.Backend.Tabels.MemberRefTabelRow[numb - 1]; //is the -1 needed?
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
                            OpCode = OpCodes.Call,
                            OpCodeName = "call"
                        };

                        inst.Operand = new CallMethodDataHolder() { ClassName = classs, NameSpace = Namespace, FunctionName = funcName };
                        inr.Add(inst);
                    }
                    catch { }
                }
                else if (opCode == OpCodes.Nop)
                {
                    inr.Add(new ILInstruction()
                    {
                        OpCode = opCode,
                        OpCodeName = "nop"
                    });
                }
                else if (opCode == OpCodes.Ret)
                {
                    inr.Add(new ILInstruction()
                    {
                        OpCode = opCode,
                        OpCodeName = "ret"
                    });
                }
                else if (opCode == OpCodes.Ldarg_0)
                {
                    inr.Add(new ILInstruction()
                    {
                        OpCode = opCode,
                        OpCodeName = "Ldarg_0"
                    });
                }
                else if (opCode == OpCodes.Ldc_I4_5)
                {
                    inr.Add(new ILInstruction()
                    {
                        OpCode = opCode,
                        OpCodeName = "ldc.i4.5"
                    });
                }
                else if (opCode == OpCodes.Stloc_0)
                {
                    inr.Add(new ILInstruction()
                    {
                        OpCode = opCode,
                        OpCodeName = "stloc.0"
                    });
                }
                else if (opCode == OpCodes.Ldloc_0)
                {
                    inr.Add(new ILInstruction()
                    {
                        OpCode = opCode,
                        OpCodeName = "ldloc.0"
                    });
                }
                else if (opCode == OpCodes.Add)
                {
                    inr.Add(new ILInstruction()
                    {
                        OpCode = opCode,
                        OpCodeName = "add"
                    });
                }
                else if (opCode == OpCodes.Stloc_1)
                {
                    inr.Add(new ILInstruction()
                    {
                        OpCode = opCode,
                        OpCodeName = "stloc.1"
                    });
                }
                else if (opCode == OpCodes.Ldloca_S)
                {
                    byte varNum = code[i + 1];
                    i++;

                    inr.Add(new ILInstruction()
                    {
                        OpCode = opCode,
                        OpCodeName = "Ldloca.s",
                        Operand = varNum
                    });
                }
                else if (opCode == OpCodes.Pop)
                {
                    inr.Add(new ILInstruction()
                    {
                        OpCode = opCode,
                        OpCodeName = "pop"
                    });
                }
                else if (opCode == OpCodes.Newobj)
                {
                    byte newObj = code[i + 1];
                    byte newObj2 = code[i + 2];
                    byte newObj3 = code[i + 3];
                    byte newObj4 = code[i + 4]; //token type. 10 = TypeRef

                    var numb = BitConverter.ToInt32(new byte[] { newObj, newObj2, newObj3, 0 }, 0);
                    
                    i += 4;

                    inr.Add(new ILInstruction()
                    {
                        OpCode = opCode,
                        OpCodeName = "newobj (WIP)"
                    });
                }
                else if (opCode == OpCodes.Add_Ovf)
                {
                    inr.Add(new ILInstruction()
                    {
                        OpCode = opCode,
                        OpCodeName = "and.ovf"
                    });
                }
                else if (opCode == OpCodes.Add_Ovf_Un)
                {
                    inr.Add(new ILInstruction()
                    {
                        OpCode = opCode,
                        OpCodeName = "add.ovf.un"
                    });
                }
                else if (opCode == OpCodes.And)
                {
                    inr.Add(new ILInstruction()
                    {
                        OpCode = opCode,
                        OpCodeName = "and"
                    });
                }
                else
                {
                    inr.Add(new ILInstruction() { OpCode = opCode, OpCodeName = "Unknown opcode: " + opCode });
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
