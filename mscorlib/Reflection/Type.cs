using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
    public abstract class Type
    {
        //TODO: make this abstract
        private string internal__fullname;
        private string internal__name;
        private string internal__namespace;
        public string get_FullName()
        {
            return internal__fullname; //Implemented in the CLR
        }
        public Assembly get_Assembly()
        {
            return GetAssemblyFromType(this);
        }
        public FieldInfo[] GetFields()
        {
            return InternalGetFields(this);
        }
        public FieldInfo GetField(string s)
        {
            return InternalGetField(this, s);
        }
        public static Type GetTypeFromHandle(RuntimeTypeHandle handle)
        {
            return Type_FromRefernce(handle);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static Type Type_FromRefernce(RuntimeTypeHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static Assembly GetAssemblyFromType(Type t);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static FieldInfo[] InternalGetFields(Type t);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern FieldInfo InternalGetField(Type t, string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern MethodInfo GetMethod(string name);
    }
}