using LibDotNetParser.CILApi;
using System;

namespace DotNetParaser
{
    class Program
    {
        static void Main(string[] args)
        {
            string dll = @"TestDll.dll";
            string exe = @"TestApp.exe";

            var vm = new DotNetVirtualMachine();
            vm.SetMainExe(new DotNetFile(exe));
            vm.AddDll(new DotNetFile(dll));
            vm.Start();

            Console.WriteLine("Program exited.");
            Console.ReadLine();
        }
    }
}