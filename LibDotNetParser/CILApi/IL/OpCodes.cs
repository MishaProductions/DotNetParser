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

        public const int Ldc_I4_5 = 0x1B;

        public const int Stloc_0 = 0x0A;

        public const int Ldloc_0 = 0x06;

        public const int Add = 0x58;

        public const int Stloc_1 = 0x0B;

        public const int Ldloca_S = 0x12;

        public const int Pop = 0x26;

        public const int Newobj = 0x73;
    }
}
