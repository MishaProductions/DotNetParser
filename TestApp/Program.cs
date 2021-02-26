using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("C# DotNetParser Tester");
            ;

            Console.WriteLine(" New Obj Test: ");
            new object();
            new StringBuilder();
            new sbyte();

            Console.WriteLine(" New varible Test: ");
            var x = 234;

            Console.WriteLine("Read array test");
            var a = args[0];


            Console.WriteLine("Output is : a="+x+" and first arg is "+a);
        }
    }
}
