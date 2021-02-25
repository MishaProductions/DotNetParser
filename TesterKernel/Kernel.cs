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
                foreach (var item in decompiler.Decompile())
                {
                    if (item.Operand is string)
                    {
                        Console.WriteLine(item.OpCodeName + " \"" + (string)item.Operand + "\"");
                    }
                    else if (item.Operand is CallMethodDataHolder)
                    {
                        var me = (CallMethodDataHolder)item.Operand;
                        Console.WriteLine(item.OpCodeName + " " + me.NameSpace + "." + me.ClassName + "." + me.FunctionName + "()");
                    }
                    else
                    {
                        Console.WriteLine(item.OpCodeName);
                    }
                }
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
