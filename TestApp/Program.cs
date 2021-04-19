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
                Console.WriteLine($"(1/6) Test success!");
            }
            else
            {
                Console.WriteLine("(1/6) Test failure. ");
            }
            //Inequal test
            if (Program.ClrTest() != 123)
            {
                Console.WriteLine("(2/6) Test success!");
            }
            else
            {
                Console.WriteLine("(2/6) Test failure.");
            }
            //Addition test
            var int2 = ClrTest() + 10;
            //Now, int2 should be 100
            if (int2 != 100)
            {
                Console.Write("(3/6) Add test failure! Result: ");
                Console.Write(int2);
                Console.WriteLine();
            }
            else
            {
                Console.Write("(3/6) Add test success! Result: ");
                Console.Write(int2);
                Console.WriteLine();
            }
            //Subtraction test
            var int3 = ClrTest() - 10;

            //Now, int3 should be 80
            if (int3 != 80)
            {
                Console.Write("(4/6) Subtract test failure! Result: ");
                Console.Write(int3);
                Console.WriteLine();
            }
            else
            {
                Console.Write("(4/6) Subtract test success! Result: ");
                Console.Write(int3);
                Console.WriteLine();
            }
            //Divide test
            var int4 = ClrTest() / 30;
            //Now, int4 should be 3
            if (int4 != 3)
            {
                Console.Write("(5/6) Divide test failure! Result: ");
                Console.Write(int4);
                Console.WriteLine();
            }
            else
            {
                Console.Write("(5/6) Divide test success! Result: ");
                Console.Write(int4);
                Console.WriteLine();
            }
            //Multiply test
            var int5 = ClrTest() * 3;
            //Now, int5 should be 3
            if (int5 != 270)
            {
                Console.Write("(5/6) Multiply test failure! Result: ");
                Console.Write(int5);
                Console.WriteLine();
            }
            else
            {
                Console.Write("(5/6) Multiply test success! Result: ");
                Console.Write(int5);
                Console.WriteLine();
            }
            int int6 = ClrTest();
            int6 += 1;

            if (int6 == 91)
            {
                Console.Write("(6/6) Increment test success! Result: ");
                Console.Write(int6);
                Console.WriteLine();
            }
            else
            {
                Console.Write("(6/6) Increment test failure. Result: ");
                Console.Write(int6);
                Console.WriteLine();
            }
            var i = ClrTest();
            if (100 > i)
            {
                Console.WriteLine("Test success. 100 > 90");
            }
            else
            {
                Console.WriteLine("Test failure.");
            }
            if (100 < i)
            {
                Console.WriteLine("Test failure.");
            }
            else
            {
                Console.WriteLine("Test success.");
            }
            for (int i2 = 0; i2 < 100; i2++)
            {
                Console.WriteLine(i2);
            }
        }
        /// <summary>
        /// Returns 90.
        /// </summary>
        /// <returns>90</returns>
        public static int ClrTest() { return 90; }
    }
}
