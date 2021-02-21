using System;
using System.Collections.Generic;
using System.Text;

namespace LibDotNetParser.CILApi.IL
{
    /// <summary>
    /// List of CIL Opcodes
    /// </summary>
    public static class OpCodes
    {
        /// <summary>
        /// No Operating
        /// </summary>
        public const int Nop = 0x0;
        /// <summary>
        /// Call function
        /// </summary>
        public const int Call = 0x28;
        /// <summary>
        /// Push string arg to Arg stack
        /// </summary>
        public const int Ldstr = 0x72;
        /// <summary>
        /// Return
        /// </summary>
        public const int Ret = 0x2A;
        /// <summary>
        /// Loads the argument at index 0 onto the evaluation stack.
        /// </summary>
        public const int Ldarg_0 = 0x02;
    }
}
