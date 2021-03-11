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
                    output += $"{item.OpCodeName} \"{@string}\"\n";
                }
                else if (item.Operand is InlineMethodOperandData me)
                {
                    output += $"{item.OpCodeName} {me.NameSpace}.{me.ClassName}.{me.FunctionName}()\n";
                }
                else if (item.Operand is int i)
                {
                    output += $"{item.OpCodeName} {i}\n";
                }
                else
                {
                    output += $"{item.OpCodeName}\n";
                }
            }
            return output;
        }
    }
}
