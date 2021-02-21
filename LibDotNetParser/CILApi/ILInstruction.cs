using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace LibDotNetParser.CILApi
{
    /// <summary>
    /// Represenets an IL Instruction
    /// </summary>
    public class ILInstruction
    {
        /// <summary>
        /// The opcode
        /// </summary>
        public int OpCode { get; set; }
        /// <summary>
        /// The operand
        /// </summary>
        public object Operand { get; set; }
        /// <summary>
        /// Extra data from decompiler
        /// </summary>
        public object DecompilerExtraData { get; set; }
    }
}
