using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("C# DotNetParser Tester");
            Console.WriteLine("Calling function");
            main2();
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void ClrHello();

        public static void main2()
        {
            Console.WriteLine("Function was called!");
            ClrHello();
        }
    }
}
