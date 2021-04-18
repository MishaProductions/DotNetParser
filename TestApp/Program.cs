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

            //Equal test
            if (ClrTest() == 123)
            {
                Console.WriteLine("(1/2) Test success!");
            }
            else
            {
                Console.WriteLine("(1/2) Test failure. ");
            }
            Console.WriteLine("Starting test 2");

            //Inequal test
            if (ClrTest() != 90)
            {
                Console.WriteLine("(1/2) Test success!");
            }
            else
            {
                Console.WriteLine("(1/2) Test failure.");
            }
        }
        public static int ClrTest() { return 123; }
    }
}
