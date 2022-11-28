//Uncomment this if you want to run the tests on the normal .NET runtime
//#define NoInternalCalls

using TestApp.Tests;
#if NoInternalCalls
using System;
#else
using System.Runtime.CompilerServices;
#endif

namespace TestApp
{
    public static class TestController
    {
#if NoInternalCalls
        static int Sucessed = 0;
        static int Failed = 0;
        public static void TestSuccess(string name)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Test success: ");
            Console.Write(name);
            Console.WriteLine();
            Console.ForegroundColor = old;
            Sucessed++;
        }

        public static void TestFail(string name)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Test failure: ");
            Console.WriteLine(name);
            Console.ForegroundColor = old;
            Failed++;
        }

        public static void TestsComplete()
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("All tests are complete");
            Console.WriteLine("Passed tests: "+Sucessed);
            Console.WriteLine("Failed tests: " + Failed);
            Console.ForegroundColor = old;
        }

        public static TestObject TestsRxObject()
        {
            return new("value");
        }
#else

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void TestSuccess(string TestName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void TestFail(string TestName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void TestsComplete();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern TestObject TestsRxObject();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void DebuggerBreak();
#endif
        public static void TestAssert(bool test, string info)
        {
            if (test) TestSuccess($"{info} works correctly");
            else TestFail($"{info} works incorrectly");
        }
    }
}
