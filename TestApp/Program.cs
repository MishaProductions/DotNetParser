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
            Console.WriteLine("Starting test 1");
            if (ClrTest() == 123)
            {
                Console.WriteLine("(1/2) Test success!");
            }
            else
            {
                Console.WriteLine("(1/2) Test failure. ");
            }
            Console.WriteLine("Starting test 2");
            if (ClrTest() != 90)
            {
                Console.WriteLine("(1/2) Test failure!");
            }
            else
            {
                Console.WriteLine("(1/2) Test success.");
            }
        }
        public static extern int ClrTest();// { return 90; }
    }
}
