using System;
using System.Collections.Generic;
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
    }
}
