using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using libDotNetClr;
using LibDotNetParser;
using LibDotNetParser.CILApi;
using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using Sys = Cosmos.System;

namespace TesterKernel
{
    public class Kernel : Sys.Kernel
    {
        private static int NumbOfSuccesssTests = 0;
        private static int NumbOfFailedTests = 0;
        private static DotNetClr clr;
        private static DotNetFile fl;
        protected override void BeforeRun()
        {
            //Init the file system
            try
            {
                fl = new DotNetFile(Builtin.TestApp);
                clr = new DotNetClr(fl);

                //Register our internal methods
                clr.RegisterResolveCallBack(AssemblyCallback);
                clr.RegisterCustomInternalMethod("TestsComplete", TestsComplete);
                clr.RegisterCustomInternalMethod("TestSuccess", TestSuccess);
                clr.RegisterCustomInternalMethod("TestFail", TestFail);
                clr.RegisterCustomInternalMethod("TestsRxObject", TestRxObject);

                clr.Start();
                Console.WriteLine("Program exec complete.");
            }
            catch (Exception x)
            {
                Console.WriteLine("Caught error: " + x.Message);
            }
        }

        protected override void Run()
        {

        }
        private static byte[] AssemblyCallback(string dll)
        {
            if (dll == "System.Private.CoreLib")
            {
                return Builtin.CorLib;
            }
            else
            {
                return null;
            }
        }
        private static void TestSuccess(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var testName = (string)Stack[Stack.Length - 1].value;

            PrintWithColor("Test Success: " + testName, ConsoleColor.Green);
            NumbOfSuccesssTests++;
        }

        private static void TestsComplete(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            Console.WriteLine();
            PrintWithColor("All Tests Completed.", ConsoleColor.DarkYellow);
            Console.WriteLine();
            PrintWithColor("Passed tests: " + NumbOfSuccesssTests, ConsoleColor.Green);
            PrintWithColor("Failed tests: " + NumbOfFailedTests, ConsoleColor.Red);
        }

        private static void TestFail(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var testName = (string)Stack[Stack.Length - 1].value;

            PrintWithColor("Test Failure: " + testName, ConsoleColor.Red);
            NumbOfFailedTests++;
        }
        private static void TestRxObject(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var cctor = fl.GetMethod("TestApp.Tests", "TestObject", ".ctor");
            if (cctor == null)
                throw new NullReferenceException();
            var s = new CustomList<MethodArgStack>();
            s.Add(MethodArgStack.String("value"));
            returnValue = clr.CreateObject(cctor, s);
        }
        private static void PrintWithColor(string text, ConsoleColor fg)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = fg;
            Console.WriteLine(text);
            Console.ForegroundColor = old;
        }
    }
}
