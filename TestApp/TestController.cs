//Uncomment this if you want to run the tests on the normal CLR
//#define NoInternalCalls
using System.Runtime.CompilerServices;
using TestApp;
using TestApp.Tests;

namespace TestApp
{
    public static class TestController
    {
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
