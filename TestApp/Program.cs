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
            if (ClrTest() == 90)
            {
                Console.WriteLine("(1/4) Test success!");
            }
            else
            {
                Console.WriteLine("(1/4) Test failure. ");
            }
            Console.WriteLine("Starting test 2");
            //Inequal test
            if (Program.ClrTest() != 123)
            {
                Console.WriteLine("(2/4) Test success!");
            }
            else
            {
                Console.WriteLine("(2/4) Test failure.");
            }
            Console.WriteLine("Starting test 3 (addition)");
            var int2 = ClrTest() + 10;

            //Now, int2 should be 100
            if (int2 != 100)
            {
                Console.Write("(3/4) Add test failure! Result: ");
                Console.Write(int2);
                Console.WriteLine();
            }
            else
            {
                Console.Write("(3/4) Add test success! Result: ");
                Console.Write(int2);
                Console.WriteLine();
            }

            Console.WriteLine("Starting test 4 (subtraction)");
            var int3 = ClrTest() - 10;

            //Now, int2 should be 100
            if (int3 != 80)
            {
                Console.Write("(4/4) Subtract test failure! Result: ");
                Console.Write(int2);
                Console.WriteLine();
            }
            else
            {
                Console.Write("(4/4) Subtract test success! Result: ");
                Console.Write(int2);
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Returns 90.
        /// </summary>
        /// <returns>90</returns>
        public static int ClrTest() { return 90; }
    }
}
