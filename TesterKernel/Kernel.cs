using Cosmos.System.FileSystem;
using LibDotNetParser;
using LibDotNetParser.CILApi;
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
                var vm = new DotNetVirtualMachine();
                vm.SetMainExe(fl);
                vm.Start();

                Console.WriteLine("Program exited.");
                Console.ReadLine();
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
