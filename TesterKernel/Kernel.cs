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
        protected override void BeforeRun()
        {
            //Init
            var fs = new Sys.FileSystem.CosmosVFS();
            VFSManager.RegisterVFS(fs);
            Console.Clear();

            try
            {
                if (!Directory.Exists(@"0:\framework"))
                    Directory.CreateDirectory(@"0:\framework");
            }
            catch (Exception x)
            {
                Console.WriteLine("Caught: " + x.Message);
            }

            try
            {
                var fl = new DotNetFile(TestApp.file);

                var decompiler = new IlDecompiler(fl.EntryPoint);
                Console.WriteLine("Decompiltion of Main function:");
                Console.WriteLine("");

                var ilFormater = new ILFormater(decompiler.Decompile());
                var outputString = ilFormater.Format();
                Console.WriteLine(outputString);
                Console.WriteLine();
                Console.WriteLine("Running program:");


                var clr = new DotNetClr(fl, @"0:\framework");
                clr.Start();
                Console.WriteLine("Program exec complete.");
            }
            catch(Exception x)
            {
                Console.WriteLine("Caught error: "+x.Message);
            }
        }

        protected override void Run()
        {
            
        }
    }
}
