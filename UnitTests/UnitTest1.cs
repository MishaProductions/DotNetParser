using libDotNetClr;
using LibDotNetParser;
using LibDotNetParser.CILApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void MainTest()
        {
            DoTest();
            if (NumbOfFailedTests > 0)
            {
                Console.WriteLine("Tests have failed, exiting now");
                Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(true);
            }
        }


        static int NumbOfSuccesssTests = 0;
        static int NumbOfFailedTests = 0;
        static void DoTest()
        {
            string exe = @"TestApp.dll";
            var m = new DotNetFile(exe);

            var decompiler = new IlDecompiler(m.EntryPoint);
            Console.WriteLine("Decompile of Main function:");
            var ilFormater = new ILFormater(decompiler.Decompile());
            var outputString = ilFormater.Format();

            Console.WriteLine(outputString);
            Console.WriteLine("Running program:");
            DotNetClr clr = new DotNetClr(
                m,
                Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),
                "framework"));

            //Register our internal methods
            clr.RegisterCustomInternalMethod("TestsComplete", TestsComplete);
            clr.RegisterCustomInternalMethod("TestSuccess", TestSuccess);
            clr.RegisterCustomInternalMethod("TestFail", TestFail);

            clr.Start();
        }
        private static void TestSuccess(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var testName = (string)Stack[Stack.Length - 1].value; //Read the 1st argument and cast it to a string

            PrintWithColor("Test Success: " + testName, ConsoleColor.Green);
            NumbOfSuccesssTests++;
        }

        private static void TestsComplete(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            if (NumbOfFailedTests == 0)
                PrintWithColor("All tests are complete. Successed tests: " + NumbOfSuccesssTests + ", failed Tests: " + NumbOfFailedTests, ConsoleColor.Green);
            else
                PrintWithColor("All tests are complete. Successed tests: " + NumbOfSuccesssTests + ", failed Tests: " + NumbOfFailedTests, ConsoleColor.Red);
        }

        private static void TestFail(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var testName = (string)Stack[Stack.Length - 1].value;

            PrintWithColor("Test Failure: " + testName, ConsoleColor.Red);
            NumbOfFailedTests++;
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
