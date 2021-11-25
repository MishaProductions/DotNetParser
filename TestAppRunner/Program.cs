using libDotNetClr;
using LibDotNetParser;
using LibDotNetParser.CILApi;
using LibDotNetParser.CILApi.IL;
using System;
using System.IO;

namespace DotNetParserRunner
{
    class Program
    {
        private static int NumbOfSuccesssTests = 0;
        private static int NumbOfFailedTests = 0;
        static void Main()
        {
            bool doil2cpu = false;
            string il2cpu = @"C:\Users\Misha\AppData\Roaming\Cosmos User Kit\Build\IL2CPU\IL2CPU.dll";
            string exe = doil2cpu ? il2cpu : "TestApp.dll";//il2cpu;
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

            clr.Start(new string[] { "testArg"});
            //Console.ReadLine();
            if (NumbOfFailedTests >= 1)
                Environment.Exit(1);
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
            PrintWithColor("Passed tests: "+NumbOfSuccesssTests, ConsoleColor.Green);
            PrintWithColor("Failed tests: " + NumbOfFailedTests, ConsoleColor.Red);
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