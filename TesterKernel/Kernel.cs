using Cosmos.System.FileSystem;
using LibDotNetParser;
using LibDotNetParser.CILApi;
using LibDotNetParser.CILApi.IL;
using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

namespace TesterKernel
{
    public class Kernel : Sys.Kernel
    {
        protected override void BeforeRun()
        {
            CosmosVFS fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            Console.Clear();
            Console.WriteLine("Cosmos booted successfully.");

            try
            {
                var fl = new DotNetFile(TestApp.file);

                var decompiler = new IlDecompiler(fl.EntryPoint);
                Console.WriteLine("Decompiltion of Main function:");
                Console.WriteLine("");
                var ilFormater = new ILFormater(decompiler.Decompile());
                var outputString = ilFormater.Format();
                Console.WriteLine(outputString);
            }
            catch(Exception x)
            {
                Console.WriteLine("Caught: "+x.Message);
            }
        }

        protected override void Run()
        {
            Console.Write("Input: ");
            var input = Console.ReadLine();
            Console.Write("Text typed: ");
            Console.WriteLine(input);
        }
    }
}
