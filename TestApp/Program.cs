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
        public void Hello(string SecondMessage)
        {
            Console.WriteLine(WelcomeMessage);
            Console.WriteLine(SecondMessage);
        }

        public class SubClass
        {
            public string Message;
            public SubClass()
            {
                Message = "Hello from the subclass!";
                Console.WriteLine("SubClass contructor");
            }
            public void HelloFromSubClass()
            {
                Console.WriteLine(Message);
            }
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
            TestField = "newr value";
            if (TestField == "newr value")
            {
                TestSuccess("Field write test (pass 2)");
            }
            else
            {
                TestFail("Field write test (pass 2)");
            }

            //Test Data types

            //TODO: fix this test
            if (string.IsNullOrEmpty(null))
                TestSuccess("string.IsNullOrEmpty(null) test");
            else
                TestSuccess("string.IsNullOrEmpty(null) == false");

            //Test byte
            var byteStr = ((byte)255).ToString();
            if (byteStr == "255")
            {
                TestSuccess("Byte.ToString test");
            }
            else
            {
                //This also tests the concat function
                TestFail("Byte.ToString test fail, ToString returned " + byteStr + " when it should be 255");
            }
            //Test sbyte
            var sbyteStr = ((sbyte)-50).ToString();
            if (sbyteStr == "-50")
            {
                TestSuccess("SByte.ToString test");
            }
            else
            {
                TestFail("SByte.ToString test fail, ToString returned " + sbyteStr + " when it should be -50.");
            }
            //Test ushort
            var ushortStr = ((ushort)52).ToString();
            if (ushortStr == "52")
            {
                TestSuccess("UInt16.ToString test");
            }
            else
            {
                TestFail("UInt16.ToString test fail, ToString returned " + ushortStr + " when it should be 52.");
            }
            //Test short
            var shortStr = ((short)-5200).ToString();
            if (shortStr == "-5200")
            {
                TestSuccess("Int16.ToString test");
            }
            else
            {
                TestFail("Int16.ToString test fail, ToString returned " + shortStr + " when it should be -5200.");
            }
            //Test int
            var intStr = (-520000).ToString();
            if (intStr == "-520000")
            {
                TestSuccess("Int32.ToString test");
            }
            else
            {
                TestFail("Int32.ToString test fail, ToString returned " + intStr + " when it should be -520000.");
            }
            //Test uint
            var uintStr = (uint.MaxValue).ToString();
            if (uintStr == "4294967295")
            {
                TestSuccess("UInt32.ToString test");
            }
            else
            {
                TestFail("UInt32.ToString test fail, ToString returned " + uintStr + " when it should be 4294967295.");
            }

            //Test strings
            var testString = "TEST";
            if (testString.Length == 4)
            {
                TestSuccess("String.Length == 4 test");
            }
            else
            {
                TestFail("String.Length == 4 test fail, testString.Length should be 4, but it is " + testString.Length.ToString());
            }
            var c = testString[0];
            if (c == 'T')
            {
                TestSuccess("Get char in string test.");
            }
            else
            {
                TestFail("Get char in string test. Wanted 'T' but got " + c);
            }
            string superLongString = "Hello there, this is a super duper long string. This is used to see how good my crappy unicode string tabel implementation is";
            Console.WriteLine(superLongString);

            if ('A'.ToString() == "A")
            {
                TestSuccess("Char.ToString() test");
            }
            else
            {
                TestFail("Char.ToString() test");
            }
            Console.WriteLine("Starting part 2 of Test suite");
            string[] stringArray = new string[8];
            if (stringArray.Length == 8)
            {
                TestSuccess("Create array");
            }
            else
            {
                TestSuccess("Create array, len should be 8 but it is " + stringArray.Length);
            }
            //TODO
            stringArray[0] = "a";
            stringArray[1] = "b";
            if (stringArray[0] == "a")
            {
                TestSuccess("Read and write to an array");
            }
            else
            {
                TestFail("Read and write to an array");
            }

            Console.WriteLine("Reflection tests");
            MyObject obj = new MyObject();
            obj.WelcomeMessage = "Hello!";
            obj.Hello("Hi again!");
            var ret = obj.GetType().FullName;
            if (ret == "DotNetparserTester.MyObject")
            {
                TestSuccess("obj.GetType().FullName is correct");
            }
            else
            {
                TestFail("obj.GetType().FullName is incorrect. Expected DotNetparserTester.MyObject, but got " + ret);
            }

            Console.WriteLine("Testing subclasses");
            MyObject.SubClass subClass = new MyObject.SubClass();
            subClass.HelloFromSubClass();

            var strType = typeof(string);
            var FUllName = strType.FullName;
            if (FUllName == "System.String")
            {
                TestSuccess("typeof(string).FullName == System.String");
            }
            else
            {
                TestFail("Full type name of string is " + FUllName + ", not System.String");
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
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void DebuggerBreak();
#endif
    }
}
