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
            Console.WriteLine("End of program");
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void ClrHello();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void ClrDispose(int a);

        public static void main2()
        {
            Console.WriteLine("Function was called!");
            ClrHello();

            //very complex math
            int a = 4 * 4 * 100 * 10345/10+932-18;
            ClrDispose(a);

            string s = "hello string world";
            s += "!";
            Console.WriteLine(s);
        }
    }
}
