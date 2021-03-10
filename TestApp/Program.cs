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

            Console.WriteLine(" New Obj Test: ");
            var obj = new object();
            if (obj == null)
                Console.WriteLine("new object() returns null!");

            Console.WriteLine(" New varible Test: ");
            var x = 234;
            if (x != 234)
                Console.WriteLine("Value of the varible is " + x + " but it should be 234");

            Console.WriteLine("Misc. Tests");
            new MySystemTime();
            int fivePlus5 = 5 + 5;
            Console.WriteLine("5 + 5 is " + fivePlus5);

            fivePlus5 -= fivePlus5;
            Console.WriteLine("subtracting 10-10 is " + fivePlus5);

            var x2 = new tests();

            Console.WriteLine("You should see a test message below this message");

            x2.test();

            Console.WriteLine("Basic .NET clr test complete");

            //if ((6 + 6) == 13)
            ClrHello();
        }
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.InternalCall)]
        private extern static void ClrHello();
        [DllImport("user32.dll")]
        public static extern void MessageBox(IntPtr handle, string text);
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
