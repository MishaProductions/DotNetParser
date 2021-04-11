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

            Console.WriteLine("While true Statement test:");
            //for (int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine("a");
            //}
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void ClrHello();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void ClrDispose(int a);

        public static void main2()
        {
            Console.WriteLine(GetBig());
           
        }

        private static byte GetBig() { return 255; }
    }
}
