using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CSharpOpcodes
{
    class Program
    {
        //tool used to generate LibDotNetParser/CILApi/IL/OpCodes/OpCodes.cs
        static void Main(string[] args)
        {
            StringBuilder singleOpCodeBuilder = new StringBuilder();
            singleOpCodeBuilder.AppendLine("// Single opcodes");
            singleOpCodeBuilder.AppendLine("public static readonly OpCode[] SingleOpCodes = new OpCode[256]");
            singleOpCodeBuilder.AppendLine("{");
            OpCode[] multiByteOpCodes = new OpCode[0x100];
            OpCode[] singleByteOpCodes = new OpCode[0x100];

            //Init
            FieldInfo[] infoArray1 = typeof(OpCodes).GetFields();
            for (int num1 = 0; num1 < infoArray1.Length; num1++)
            {
                FieldInfo info1 = infoArray1[num1];
                if (info1.FieldType == typeof(OpCode))
                {
                    OpCode code1 = (OpCode)info1.GetValue(null);
                    ushort num2 = (ushort)code1.Value;
                    if (num2 < 0x100)
                    {
                        singleByteOpCodes[(int)num2] = code1;
                    }
                    else
                    {
                        if ((num2 & 0xff00) != 0xfe00)
                        {
                            throw new Exception("Invalid OpCode.");
                        }
                        multiByteOpCodes[num2 & 0xff] = code1;
                    }
                }
            }

            //Dump single opcodes
            for (int i = 0; i < singleByteOpCodes.Length; i++)
            {
                var op = singleByteOpCodes[i];
                if (op == null)
                {
                    singleOpCodeBuilder.AppendLine("null,");
                }
                else if (string.IsNullOrEmpty(op.Name))
                {
                    singleOpCodeBuilder.AppendLine("null,");
                }
                else
                {
                    var val = op.Value;//.ToString("X");
                    singleOpCodeBuilder.AppendLine("new OpCode(\"" + op.Name + "\", " + val + ", OpCodeOperandType." + op.OperandType.ToString() + "),");
                }
            }
            singleOpCodeBuilder.AppendLine("};");

            singleOpCodeBuilder.AppendLine();
            singleOpCodeBuilder.AppendLine();
            singleOpCodeBuilder.AppendLine("// Multi opcodes");
            //Dump multi opcodes
            StringBuilder multiOpCodeBuilder = new StringBuilder();
            multiOpCodeBuilder.AppendLine("public static readonly OpCode[] MultiOpCodes = new OpCode[256]");
            multiOpCodeBuilder.AppendLine("{");
            for (int i = 0; i < multiByteOpCodes.Length; i++)
            {
                var op = multiByteOpCodes[i];
                if (op == null)
                {
                    multiOpCodeBuilder.AppendLine("null,");
                }
                else if (string.IsNullOrEmpty(op.Name))
                {
                    multiOpCodeBuilder.AppendLine("null,");
                }
                else
                {
                    var val = op.Value + 512;//.ToString("X");
                    multiOpCodeBuilder.AppendLine("new OpCode(\"" + op.Name + "\", " + val + ", OpCodeOperandType." + op.OperandType.ToString() + ", true),");
                }
            }
            multiOpCodeBuilder.AppendLine("};");

            multiOpCodeBuilder.AppendLine();
            multiOpCodeBuilder.AppendLine();
            multiOpCodeBuilder.AppendLine("// OpCode enum");
            multiOpCodeBuilder.AppendLine("public enum OpCodesList : ushort");
            multiOpCodeBuilder.AppendLine("{");
            foreach (var item in infoArray1.OrderBy(c => c.Name))
            {
                var opcode = (OpCode)item.GetValue(null);
                multiOpCodeBuilder.AppendLine($"{item.Name} = {(ushort)opcode.Value},");
            }
            multiOpCodeBuilder.AppendLine("}");
            //Dump multi opcodes

            File.WriteAllText("opcodes.cs", singleOpCodeBuilder.ToString() + Environment.NewLine + multiOpCodeBuilder.ToString());


            Console.WriteLine("Write complete. Press enter to exit.");
            Console.ReadLine();
        }
    }
}
