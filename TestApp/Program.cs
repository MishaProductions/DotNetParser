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
    class MyObject
    {
        public string WelcomeMessage;
        public MyObject()
        {
            WelcomeMessage = "lol";
            Console.WriteLine("constructor end....");
        }
        public void Hello()
        {
            Console.WriteLine(WelcomeMessage);
        }
    }
    class Program
    {
        public static string TestField = "Default Value.";
        public const string ConstString = "Constant String.";
        static void Main(string[] args)
        {
            //Equal test
            if (ClrTest() == 90)
            {
                Console.WriteLine($"(1/6) Equal Test success!");
            }
            else
            {
                Console.WriteLine("(1/6) Equal Test failure. ");
            }
            //Inequal test
            if (Program.ClrTest() != 123)
            {
                Console.WriteLine("(2/6) Inequal Test success!");
            }
            else
            {
                Console.WriteLine("(2/6) Inequal Test failure.");
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
            int i2;
            for (i2 = 0; i2 < 100; i2++)
            {
                Console.Write(i2);
                Console.Write(" ");
            }
            if (i2 == 100)
            {
                Console.WriteLine();
                Console.WriteLine("forloop test success");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("forloop test fail");
                Console.WriteLine(i2);
            }

            Console.WriteLine(TestField);

            if (TestField == "Default Value.")
            {
                Console.WriteLine("Field test success");
            }
            else
            {
                Console.WriteLine("Field test failure");
            }
            TestField = "new value";
            if (TestField == "new value")
            {
                Console.WriteLine("Field write test success");
            }
            else
            {
                Console.WriteLine("Field write test failure");
            }
            MyObject obj = new MyObject();
            obj.WelcomeMessage = "Hello!";
            obj.Hello();
        }
        /// <summary>
        /// Returns 90.
        /// </summary>
        /// <returns>90</returns>
        public static int ClrTest() { return 90; }
    }
}
