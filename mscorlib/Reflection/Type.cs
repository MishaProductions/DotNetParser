using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
    public abstract class Type
    {
        //TODO: make this abstract
        private string internal__fullname = "Error! value was never assigned.";
        private string internal__name = "Error! value was never assigned.";
        private string internal__namespace = "Error! value was never assigned.";
        public string get_FullName()
        {
            return internal__fullname; //Implemented in the CLR
        }
        public string get_Name()
        {
            return internal__name; //Implemented in the CLR
        }
        public string get_Namespace()
        {
            return internal__namespace; //Implemented in the CLR
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