using LibDotNetParser.CILApi;
using LibDotNetParser.CILApi.IL;
using System;

namespace DotNetParaser
{
    class Program
    {
        static void Main()
        {
            //string dll = @"TestDll.dll";
            string exe = @"TestApp.exe";
            var m = new DotNetFile(exe);

            var decompiler = new IlDecompiler(m.EntryPoint);
            Console.WriteLine("Decompiltion of Main function:");
            Console.WriteLine("");
            foreach (var item in decompiler.Decompile())
            {
                if (item.Operand is string @string)
                {
                    Console.WriteLine(item.OpCodeName+" \""+@string+"\"");
                }
                else if (item.Operand is CallMethodDataHolder me)
                {
                    Console.WriteLine(item.OpCodeName + " " + me.NameSpace + "." + me.ClassName + "." + me.FunctionName + "()");
                }
                else
                {
                    Console.WriteLine(item.OpCodeName);
                }
            }
            Console.WriteLine("Program exited.");
            Console.ReadLine();
        }
    }
}