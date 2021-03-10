using LibDotNetParser;
using LibDotNetParser.CILApi;
using LibDotNetParser.CILApi.IL;
using System;
using System.IO;
using static DotNetClr.DotNetClr;

namespace DotNetParaser
{
    class Program
    {
        static void Main()
        {
            string exe = @"TestApp.exe";
            var m = new DotNetFile(exe);

            var decompiler = new IlDecompiler(m.EntryPoint);
            Console.WriteLine("Decompile of Main function   :");
            Console.WriteLine("==============================");
            var ilFormater = new ILFormater(decompiler.Decompile());
            var outputString = ilFormater.Format();

            Console.WriteLine(outputString);
            Console.WriteLine("Running program              :");
            Console.WriteLine("==============================");
            DotNetClr.DotNetClr clr = new DotNetClr.DotNetClr(
                m,
                Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),
                "framework"));
            clr.Start();


            Console.WriteLine("Program exited.");
            Console.ReadLine();
        }
    }
}