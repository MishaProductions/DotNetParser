using LibDotNetParser.CILApi;
using System;

namespace DotNetParaser
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = @"TestDll.dll";

            var vm = new DotNetVirtualMachine(new DotNetFile(file));
            vm.Start();

            Console.WriteLine("Program exited.");
            Console.ReadLine();
        }
    }
}