using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        public event EventHandler test;
        public const string MyConstant = "My Constant!!!!";
        [SecurityPermissionAttribute(SecurityAction.Deny)]
        [SecurityCriticalAttribute()]
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

    [StructLayout(LayoutKind.Explicit, Size = 16, CharSet = CharSet.Ansi)]
    public class MySystemTime
    {
        [FieldOffset(0)] public ushort wYear;
        [FieldOffset(2)] public ushort wMonth;
        [FieldOffset(4)] public ushort wDayOfWeek;
        [FieldOffset(6)] public ushort wDay;
        [FieldOffset(8)] public ushort wHour;
        [FieldOffset(10)] public ushort wMinute;
        [FieldOffset(12)] public ushort wSecond;
        [FieldOffset(14)] public ushort wMilliseconds;
    }
}
