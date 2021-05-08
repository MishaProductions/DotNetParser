//Uncomment this if you want to run the tests on the normal CLR
//#define NoInternalCalls
using System;
using System.Runtime.CompilerServices;

namespace DotNetparserTester
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
            ////Equal test
            if (ClrTest() == 90)
            {
                TestSuccess("Equal Test");
            }
            else
            {
                TestFail("Equal Test");
            }
            //Inequal test
            if (Program.ClrTest() != 123)
            {
                TestSuccess("Inequal Test");
            }
            else
            {
                TestFail("Inequal Test");
            }
            //Addition test
            var int2 = ClrTest() + 10;
            //Now, int2 should be 100
            if (int2 != 100)
            {
                TestFail("Add test");
                Console.Write("(3/6) Add test failure! Result: ");
                Console.Write(int2);
                Console.WriteLine();
            }
            else
            {
                TestSuccess("Add test");
            }
            //Subtraction test
            var int3 = ClrTest() - 10;

            //Now, int3 should be 80
            if (int3 != 80)
            {
                TestFail("Subtract test");
                Console.Write("(4/6) Subtract test failure! Result: ");
                Console.Write(int3);
                Console.WriteLine();
            }
            else
            {
                TestSuccess("Subtract test");
            }
            //Divide test
            var int4 = ClrTest() / 30;
            //Now, int4 should be 3
            if (int4 != 3)
            {
                TestFail("Divide test");
                Console.Write("(5/6) Divide test failure! Result: ");
                Console.Write(int4);
                Console.WriteLine();
            }
            else
            {
                TestSuccess("Divide test");
            }
            //Multiply test
            var int5 = ClrTest() * 3;
            //Now, int5 should be 3
            if (int5 != 270)
            {
                TestFail("Multiply test");
                Console.Write("(5/6) Multiply test failure! Result: ");
                Console.Write(int5);
                Console.WriteLine();
            }
            else
            {
                TestSuccess("Multiply test");
            }
            int int6 = ClrTest();
            int6 += 1;

            if (int6 == 91)
            {
                TestSuccess("Increment test");
            }
            else
            {
                TestFail("Increment test");
                Console.Write("(6/6) Increment test failure. Result: ");
                Console.Write(int6);
                Console.WriteLine();
            }
            var i = ClrTest();
            if (100 > i)
            {
                TestSuccess("Greater than if statement");
            }
            else
            {
                TestFail("Greater than if statement");
            }
            if (100 < i)
            {
                TestFail("Less than if statement");
            }
            else
            {
                TestSuccess("Less than if statement");
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
                TestSuccess("For loop test");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Got for loop test result: ");
                Console.WriteLine(i2);

                TestFail("For loop test");
            }

            Console.WriteLine(TestField);

            if (TestField == "Default Value.")
            {
                TestSuccess("Field test success");
            }
            else
            {
                TestFail("Field test failure");
            }
            TestField = "new value";
            if (TestField == "new value")
            {
                TestSuccess("Field write test");
            }
            else
            {
                TestFail("Field write test");
            }
            MyObject obj = new MyObject();
            obj.WelcomeMessage = "Hello!";
            obj.Hello();
            TestField = "newr value";
            if (TestField == "newr value")
            {
                TestSuccess("Field write test (pass 2)");
            }
            else
            {
                TestFail("Field write test (pass 2)");
            }
            TestsComplete();
        }
        /// <summary>
        /// Returns 90.
        /// </summary>
        /// <returns>90</returns>
        public static int ClrTest() { return 90; }

#if NoInternalCalls
        private static void TestSuccess(string name)
        {
            Console.Write("Test success: ");
            Console.Write(name);
            Console.WriteLine();
        }

        private static void TestFail(string name)
        {
            Console.Write("Test failure: ");
            Console.WriteLine(name);
        }

        private static void TestsComplete()
        {
            Console.WriteLine("All tests are complete");
        }
#else

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void TestSuccess(string TestName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void TestFail(string TestName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void TestsComplete();
#endif
    }
}
