using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using libDotNetClr;
using LibDotNetParser;
using LibDotNetParser.CILApi;
using System;
using System.IO;
using Sys = Cosmos.System;

namespace TesterKernel
{
    public class Kernel : Sys.Kernel
    {
        private static int NumbOfSuccesssTests = 0;
        private static int NumbOfFailedTests = 0;
        protected override void BeforeRun()
        {
            //Init
            var fs = new Sys.FileSystem.CosmosVFS();
            VFSManager.RegisterVFS(fs);
            Console.Clear();

            string boot = "";
            bool frame = false;
            foreach (var item in fs.GetVolumes())
            {
                Console.WriteLine("Found volume: " + item.mFullPath);
                foreach (var d in Directory.GetDirectories(item.mFullPath))
                {
                    Console.WriteLine("dir: " + d);
                }
                foreach (var d in Directory.GetFiles(item.mFullPath))
                {
                    Console.WriteLine("file: " + d);
                }
                if (File.Exists(item.mFullPath + "TESTAPP.DLL") | File.Exists(item.mFullPath + "dotnetparser.exe"))
                {
                    Console.WriteLine("Found boot volume: " + item.mFullPath);
                    boot = item.mFullPath;
                }
                if (Directory.Exists(item.mFullPath + "framework"))
                {
                    frame = true;
                }
                else if (Directory.Exists(item.mFullPath + "FRAMEW"))
                {
                    frame = false;
                }
            }
            if (boot == "")
            {
                Console.WriteLine("Cannot find the media that we booted from.");
                return;
            }
            try
            {
                byte[] fi = TestApp.file;
                if (File.Exists(boot + @"TESTAPP.DLL"))
                {
                    fi = File.ReadAllBytes(boot + @"TESTAPP.DLL");
                }
                else if (File.Exists(boot + @"DotNetparserTester.exe"))
                {
                    fi = File.ReadAllBytes(boot + @"DotNetparserTester.exe");
                }
                var fl = new DotNetFile(fi);

                var decompiler = new IlDecompiler(fl.EntryPoint);
                Console.WriteLine("Decompiltion of Main function:");
                Console.WriteLine("");
                Console.WriteLine(boot);
                var ilFormater = new ILFormater(decompiler.Decompile());
                var outputString = ilFormater.Format();
                Console.WriteLine(outputString);
                Console.WriteLine();
                Console.WriteLine("Running program:");


                var clr = new DotNetClr(fl, frame ? boot + @"framework" : boot + @"FRAMEW");

                //Register our internal methods
                clr.RegisterCustomInternalMethod("TestsComplete", TestsComplete);
                clr.RegisterCustomInternalMethod("TestSuccess", TestSuccess);
                clr.RegisterCustomInternalMethod("TestFail", TestFail);

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

        private static void PrintWithColor(string text, ConsoleColor fg)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = fg;
            Console.WriteLine(text);
            Console.ForegroundColor = old;
        }
    }
}
