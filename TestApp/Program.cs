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

        public string MyProperty { get; set; } = "Property Default Value.";
        [SecurityCriticalAttribute()]
        static void Main(string[] args)
        {
            Console.WriteLine("C# DotNetParser Tester");
            Console.WriteLine("Calling function");
            main2();
        }
        //[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.InternalCall)]
        private extern static void ClrHello();

        public static void main2()
        {
            Console.WriteLine("Function was called!");
            ClrHello();
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 16, CharSet = CharSet.Ansi)]
    public struct MySystemTime
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

    public interface ITest { void test(); }
    public abstract class AbstractClass
    {
        public abstract void ImplementMe();
        public virtual void Virt() { }
    }
    public class Tests2 : AbstractClass
    {
        public override void ImplementMe()
        {
            Console.WriteLine("See this? good 4 u");
        }
        public override void Virt()
        {
            base.Virt();
            Console.WriteLine("lol");
        }
    }
    public class tests : ITest
    {
        public void test()
        {
            Console.WriteLine("Test!");
        }
    }
}
