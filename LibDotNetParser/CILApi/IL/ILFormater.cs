using LibDotNetParser.CILApi;
using LibDotNetParser.CILApi.IL;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibDotNetParser
{
    /// <summary>
    /// Converts ILInstruction[] to a string
    /// </summary>
    public class ILFormater
    {
        ILInstruction[] insts;
        public ILFormater(ILInstruction[] insts)
        {
            this.insts = insts;
        }

        public string Format()
        {
            string output = "";
            foreach (var item in insts)
            {
                if (item.Operand is string @string)
                {
                    output += $"IL_{item.Position}: {item.OpCodeName} \"{@string}\"\n";
                }
                else if (item.Operand is InlineMethodOperandData me)
                {
                    output += $"IL_{item.Position}: {item.OpCodeName} {me.NameSpace}.{me.ClassName}.{me.FunctionName}()\n";
                }
                else if (item.Operand is int i)
                {
                    output += $"IL_{item.Position}: {item.OpCodeName} {i}\n";
                }
                else if (item.Operand is byte b)
                {
                    output += $"IL_{item.Position}: {item.OpCodeName} {b}\n";
                }
                else
                {
                    output += $"IL_{item.Position}: {item.OpCodeName}\n";
                }
            }
            return output;
        }
    }
}
