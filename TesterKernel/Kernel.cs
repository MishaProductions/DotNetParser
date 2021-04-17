using Cosmos.System.FileSystem;
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
            CosmosVFS fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
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


                DotNetClr.DotNetClr clr = new DotNetClr.DotNetClr(fl, @"0:\framework");
                clr.Start();
                Console.WriteLine("Program exec complete.");
            }
            catch(Exception x)
            {
                Console.WriteLine("Caught: "+x.Message);
            }
        }

        protected override void Run()
        {
            
        }
    }
}
