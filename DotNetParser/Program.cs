using LibDotNetParser.CILApi;
using LibDotNetParser.CILApi.IL;
using System;

namespace DotNetParaser
{
    class Program
    {
        static void Main(string[] args)
        {
            string dll = @"TestDll.dll";
            string exe = @"TestApp.exe";
            var m = new DotNetFile(exe);



            var decompiler = new IlDecompiler(m.EntryPoint);
            Console.WriteLine("Decompiltion of Main function:");
            Console.WriteLine("");
            foreach (var item in decompiler.Decompile())
            {
                if (item.Operand is string)
                {
                    Console.WriteLine(item.OpCodeName+" \""+(string)item.Operand+"\"");
                }
                else if (item.Operand is CallMethodDataHolder)
                {
                    var me = (CallMethodDataHolder)item.Operand;
                    Console.WriteLine(item.OpCodeName + " " + me.NameSpace+"."+me.ClassName+"."+me.FunctionName + "()");
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