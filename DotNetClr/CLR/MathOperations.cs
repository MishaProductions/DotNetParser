using LibDotNetParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libDotNetClr
{
    public static class MathOperations
    {
        public enum Operation
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Remainder,
            Equality
        }

        public static MethodArgStack Op(MethodArgStack arg1, MethodArgStack arg2, Operation op)
        {
            if (arg1.type != arg2.type) throw new Exception("Inconsistent type definitions");

            switch (arg1.type)
            {
                case StackItemType.Float32: return OpWithFloat32(arg1, arg2, op);
                case StackItemType.Float64: return OpWithFloat64(arg1, arg2, op);
                case StackItemType.Int32: return OpWithInt32(arg1, arg2, op);
                case StackItemType.Int64: return OpWithInt64(arg1, arg2, op);
                case StackItemType.ldnull: return OpWithLdNull(arg1, arg2, op);
                default: throw new NotImplementedException();
            }
        }

        public static MethodArgStack OpWithFloat32(MethodArgStack arg1, MethodArgStack arg2, Operation op)
        {
            float v1 = (float)arg1.value;
            float v2 = (float)arg2.value;

            switch (op)
            {
                case Operation.Add: return MethodArgStack.Float32(v1 + v2);
                case Operation.Subtract: return MethodArgStack.Float32(v1 - v2);
                case Operation.Multiply: return MethodArgStack.Float32(v1 * v2);
                case Operation.Divide: return MethodArgStack.Float32(v1 / v2);
                case Operation.Remainder: return MethodArgStack.Float32(v1 % v2);
                case Operation.Equality: return MethodArgStack.Int32(v1 == v2 ? 1 : 0);
                default: throw new Exception("Invalid operation");
            }
        }

        public static MethodArgStack OpWithFloat64(MethodArgStack arg1, MethodArgStack arg2, Operation op)
        {
            double v1 = (double)arg1.value;
            double v2 = (double)arg2.value;

            switch (op)
            {
                case Operation.Add: return MethodArgStack.Float64(v1 + v2);
                case Operation.Subtract: return MethodArgStack.Float64(v1 - v2);
                case Operation.Multiply: return MethodArgStack.Float64(v1 * v2);
                case Operation.Divide: return MethodArgStack.Float64(v1 / v2);
                case Operation.Remainder: return MethodArgStack.Float64(v1 % v2);
                case Operation.Equality: return MethodArgStack.Int32(v1 == v2 ? 1 : 0);
                default: throw new Exception("Invalid operation");
            }
        }

        public static MethodArgStack OpWithInt32(MethodArgStack arg1, MethodArgStack arg2, Operation op)
        {
            int v1 = (int)arg1.value;
            int v2 = (int)arg2.value;

            switch (op)
            {
                case Operation.Add: return MethodArgStack.Int32(v1 + v2);
                case Operation.Subtract: return MethodArgStack.Int32(v1 - v2);
                case Operation.Multiply: return MethodArgStack.Int32(v1 * v2);
                case Operation.Divide: return MethodArgStack.Int32(v1 / v2);
                case Operation.Remainder: return MethodArgStack.Int32(v1 % v2);
                case Operation.Equality: return MethodArgStack.Int32(v1 == v2 ? 1 : 0);
                default: throw new Exception("Invalid operation");
            }
        }

        public static MethodArgStack OpWithInt64(MethodArgStack arg1, MethodArgStack arg2, Operation op)
        {
            long v1 = (long)arg1.value;
            long v2 = (long)arg2.value;

            switch (op)
            {
                case Operation.Add: return MethodArgStack.Int64(v1 + v2);
                case Operation.Subtract: return MethodArgStack.Int64(v1 - v2);
                case Operation.Multiply: return MethodArgStack.Int64(v1 * v2);
                case Operation.Divide: return MethodArgStack.Int64(v1 / v2);
                case Operation.Remainder: return MethodArgStack.Int64(v1 % v2);
                case Operation.Equality: return MethodArgStack.Int32(v1 == v2 ? 1 : 0);
                default: throw new Exception("Invalid operation");
            }
        }

        public static MethodArgStack OpWithLdNull(MethodArgStack arg1, MethodArgStack arg2, Operation op)
        {
            object v1 = arg1.value;
            object v2 = arg2.value;

            switch (op)
            {
                case Operation.Equality: return MethodArgStack.Int32(v1 == v2 ? 1 : 0);
                default: throw new Exception("Invalid operation");
            }
        }
    }
}
