//Uncomment this if you want to run the tests on the normal CLR
//#define NoInternalCalls
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DotNetparserTester
{
    class MyObject
    {
        public string WelcomeMessage;
        public Action<string> s;
        public MyObject()
        {
            WelcomeMessage = "Second Message";
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
        public static Action<string> console;

        static void Main(string[] args)
        {
            //Equal test
            if (ClrTest() == 90)
            {
                TestSuccess("Equal Test");
            }
            else
            {
                TestFail("Equal Test");
            }
            //Inequal test
            if (ClrTest() != 123)
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
            var uintStr = uint.MaxValue.ToString();
            if (uintStr == "4294967295")
            {
                TestSuccess("UInt32.ToString test");
            }
            else
            {
                TestFail("UInt32.ToString test fail, ToString returned " + uintStr + " when it should be 4294967295.");
            }
            //Test ulong
            var ulongStr = ulong.MaxValue.ToString();
            if (ulongStr == "18446744073709551615")
            {
                TestSuccess("UInt64.ToString test");
            }
            else
            {
                TestFail("UInt64.ToString test fail, ToString returned " + ulongStr + " when it should be 18446744073709551615.");
            }

            //Test long
            //var longStr = long.MinValue.ToString();
            //if (ulongStr == "-9223372036854775808")
            //{
            //    TestSuccess("Int64.ToString test");
            //}
            //else
            //{
            //    TestFail("Int64.ToString test fail, ToString returned " + ulongStr + " when it should be -9223372036854775808.");
            //}


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
            if (testString[0] == 'T')
            {
                TestSuccess("Get char in string test.");
            }
            else
            {
                TestFail("Get char in string test. Wanted 'T' but got " + testString[0]);
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
                TestFail("Create array, len should be 8 but it is " + stringArray.Length);
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
                TestFail("Full type name of string is System.String, not " + FUllName);
            }
            var flag1 = 78 >= 76;
            if (flag1)
            {
                TestSuccess("78 >= 76");
            }
            else
            {
                TestFail("78 >= 76 returned false!");
            }
            var flag2 = 78 <= 76;
            if (flag2)
            {
                TestFail("78 <= 76 returned true!");
            }
            else
            {
                TestSuccess("78 <= 76");
            }

            Action<string> action = (string parm) => Console.WriteLine(parm);
            action.Invoke("hi there :)");
            Action<string, string> action2 = (string parm, string b) => Console.WriteLine(parm + b);
            action2.Invoke("hi there ", ":)");
            Action<string, string, string> action3 = (string parm, string b, string c) => Console.WriteLine(parm + b + c);
            action3.Invoke("hi there ", ":", ")");
            Action<string, string, string, string> action4 = (string parm, string b, string c, string d) => Console.WriteLine(parm + b + c + d);
            action4.Invoke("hi there", " ", ":", ")");
            Action<string, string, string, string, int> action5 = (string parm, string b, string c, string d, int f) =>
            {
                Console.WriteLine(parm + b + c + d);
                Console.WriteLine(f);
            };
            action5.Invoke("hi there", " ", ":", ")", 5);
            if (true.ToString() == "True")
            {
                TestSuccess("true.ToString() is true");
            }
            else
            {
                TestFail("true.toString != true");
            }
            if (Array.Empty<string>().Length == 0)
            {
                TestSuccess("Array.Empty() returns empty array");
            }
            else
            {
                TestFail("Array.Empty() returned non empty array");
            }
            Console.WriteLine("Action as argument tests");
            ActionAsMethodArgTest(new string[] { "hi", "bye" }, action, action);
            ActionAsMethodArgTest2(new string[] { "a", "b" });
            int h = 0;
            TestByRef("this is correct", ref h);
            if (h == 0)
            {
                TestFail("ByRef value did not change!");
            }
            else if (h == 1)
            {
                TestSuccess("ByRef value is correct");
            }
            else
            {
                TestFail("ByRef value is not a 1 or a 0.");
            }

            Console.WriteLine("Interface tests");
            IHelloWorldFunction f = new HelloWorldFunction();
            f.SayHello();
            Console.WriteLine(f.HelloMessage);



            var pType = typeof(Program);
            Console.WriteLine("Type of program: " + pType.FullName);
            Console.WriteLine("List tests");
            List<string> list = new List<string>();
            list.Add("item 1");
            list.Add("item 2");
            //list.RemoveAt(1);
            //list.Remove("item 1");

            //if(list.Count != 0)
            //{
            //    TestFail("List count is not zero!");
            //}
            //else
            //{
            //    TestSuccess("List count is zero");
            //}

            if (list[0] == "item 1")
            {
                TestSuccess("Adding item to list");
            }
            else
            {
                TestFail("Adding item to list failed.");
            }

            var str3 = "AB";
            if (str3.IndexOf('B') == 1)
            {
                TestSuccess("String.IndexOf() is 1");
            }
            else
            {
                TestFail("String.IndexOf() is not 1");
            }
            float fl = 0.1f;
            if (fl == 0.1f)
            {
                TestSuccess("Float is correct value");
            }
            else
            {
                TestFail("Float is incorrect value");
            }
            fl += 0.1f;
            if (fl == 0.2f)
            {
                TestSuccess("Float is correct value after adding .1");
            }
            else
            {
                TestFail("Float is incorrect value after adding .1");
            }
            Func<string> func0 = GetTestMessage;
            var val = func0();
            if (val == "Func test")
            {
                TestSuccess("Func<string> works correctly");
            }
            else
            {
                TestFail("Func<string> works incorrectly");
            }

            Func<string, string> func1 = GetTestMessage2;
            var val2 = func1("arg");
            if (val2 == "this function has 2 args")
            {
                TestSuccess("Func<string, string> works correctly");
            }
            else
            {
                TestFail("Func<string, string> works incorrectly");
            }
            fl -= 0.2f;
            if (fl == 0)
            {
                TestSuccess("Float is correct value after subtracting .2");
            }
            else
            {
                TestFail("Float is incorrect value after subtracting .2");
            }
            fl = 2.5f;
            fl *= 2;
            if (fl == 5)
            {
                TestSuccess("Float is correct value after multiplying 2.5f*2");
            }
            else
            {
                TestFail("Float is incorrect value after multiplying 2.5f*2");
            }
            fl /= 2.5f;
            if (fl == 2)
            {
                TestSuccess("Float is correct value after dividing by 2.5f");
            }
            else
            {
                TestFail("Float is incorrect value after dividing by 2.5f");
            }
            fl = 5;
            if ((int)fl == 5)
            {
                TestSuccess("float was 5 and correctly converted from float to int");
            }
            else
            {
                TestFail("float was not correctly converted to an int");
            }
            h = 3;
            fl = h;
            if ((int)fl == 3)
            {
                TestSuccess("float was 3 and correctly converted from int to float");
            }
            else
            {
                TestFail("float was not correctly converted from an int");
            }
            i = 7;
            TestAssert((i % 3) == 1, "int32 modulus");
            fl = 2.5f;
            TestAssert((fl % 1f) == 0.5f, "float32 modulus");
            i = 0x12345678;
            i2 = 0x00ff00ff;
            TestAssert((i & i2) == 0x00340078, "int32 bitwise and");
            i = 0x12345678;
            i2 = 0x0f0f0f0f;
            TestAssert((i | i2) == 0x1f3f5f7f, "int32 bitwise or");
            i = 0x12345678;
            i2 = 0x0a0a0a0a;
            TestAssert((i ^ i2) == 0x183E5C72, "int32 bitwise xor");
            uint ui = 0x12345678;
            TestAssert(~ui == 0xEDCBA987, "uint32 bitwise not");
            i = 0x12345678;
            TestAssert((i >> 4) == 0x01234567, "int32 shift right");
            i = 0x12345678;
            TestAssert((i << 4) == 0x23456780, "int32 shift left");

            TestsComplete();
        }

        private static void TestAssert(bool test, string info)
        {
            if (test) TestSuccess($"{info} works correctly");
            else TestFail($"{info} works incorrectly");
        }

        private static string GetTestMessage2(string arg)
        {
            return "this function has 2 " + arg + "s";
        }

        private static string GetTestMessage()
        {
             return "Func test";
        }

        private static void TestByRef(string t, ref int a)
        {
            Console.WriteLine("Testing ByRef. t = " + t);
            a = 1;
        }
        private static void ActionAsMethodArgTest(string[] vs, Action<string> action1, Action<string> action2)
        {
            ActionAsMethodArgTestB(vs, action2, action1);
        }
        private static void ActionAsMethodArgTestB(string[] vs, Action<string> action1, Action<string> action2)
        {
            console = action1;

            console(vs[0]);
            console(vs[1]);
        }
        private static void ActionAsMethodArgTest2(string[] vs)
        {
            console(vs[0]);
            console(vs[1]);
        }
        private static void test5(string[] v, object f, int a)
        {
            Console.WriteLine(v[a]);
        }

        /// <summary>
        /// Returns 90.
        /// </summary>
        /// <returns>90</returns>
        public static int ClrTest() { return 90; }
        public static void Test(Type t, string[] a, int b)
        {

        }
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
    public interface IHelloWorldFunction
    {
        void SayHello();
        string HelloMessage { get; set; }
    }
    public class HelloWorldFunction : IHelloWorldFunction
    {
        public string HelloMessage { get => "This is a field on an interface!"; set => throw new NotImplementedException(); }

        public void SayHello()
        {
            Console.WriteLine("Hello world from an interface!");
        }
    }
}
