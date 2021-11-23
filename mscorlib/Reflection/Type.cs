using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace System
{
    public abstract class Type
    {
        //TODO: make this abstract
        private string internal__fullname;
        public string get_FullName()
        {
            return internal__fullname; //Implemented in the CLR
        }
        public Assembly get_Assembly()
        {
            return GetAssemblyFromType();
        }

        public static Type GetTypeFromHandle(RuntimeTypeHandle handle)
        {
            return Type_FromRefernce(handle);
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static Type Type_FromRefernce(RuntimeTypeHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static Assembly GetAssemblyFromType();
    }
}