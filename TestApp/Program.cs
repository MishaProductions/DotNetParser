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
            //Equal test
            if (ClrTest() == 90)
            {
                Console.WriteLine("(1/4) Test success!");
            }
            else
            {
                Console.WriteLine("(1/4) Test failure. ");
            }
            //Inequal test
            if (Program.ClrTest() != 123)
            {
                Console.WriteLine("(2/4) Test success!");
            }
            else
            {
                Console.WriteLine("(2/4) Test failure.");
            }
            //Addition test
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
            //Subtraction test
            var int3 = ClrTest() - 10;

            //Now, int3 should be 80
            if (int3 != 80)
            {
                Console.Write("(4/4) Subtract test failure! Result: ");
                Console.Write(int3);
                Console.WriteLine();
            }
            else
            {
                Console.Write("(4/4) Subtract test success! Result: ");
                Console.Write(int3);
                Console.WriteLine();
            }
            //Divide test
            var int4 = ClrTest() / 30;
            //Now, int4 should be 3
            if (int4 != 3)
            {
                Console.Write("(5/5) Divide test failure! Result: ");
                Console.Write(int4);
                Console.WriteLine();
            }
            else
            {
                Console.Write("(5/5) Divide test success! Result: ");
                Console.Write(int4);
                Console.WriteLine();
            }
            //Multiply test
            var int5 = ClrTest() * 3;
            //Now, int5 should be 3
            if (int5 != 270)
            {
                Console.Write("(5/5) Multiply test failure! Result: ");
                Console.Write(int5);
                Console.WriteLine();
            }
            else
            {
                Console.Write("(5/5) Multiply test success! Result: ");
                Console.Write(int5);
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
