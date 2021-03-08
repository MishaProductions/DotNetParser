using LibDotNetParser;
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
            Console.WriteLine("Decompile of Main function   :");
            Console.WriteLine("==============================");
            var ilFormater = new ILFormater(decompiler.Decompile());
            var outputString = ilFormater.Format();
            Console.WriteLine(outputString);
            Console.WriteLine("Running program               :");
            Console.WriteLine("==============================");
            Console.WriteLine("Program exited.");
            Console.ReadLine();
        }
    }
}