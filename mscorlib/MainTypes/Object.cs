using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace System
{
    public class Object
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Type GetObjType();
        public Object()
        {
            
        }
        ~Object()
        {

        }

        public virtual string ToString()
        {
            return "";
        }

        public virtual Type GetType()
        {
            return GetObjType();
        }
    }
}
