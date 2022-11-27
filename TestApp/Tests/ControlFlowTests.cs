using System;

namespace TestApp.Tests
{
    /// <summary>
    /// Any tests that test the stack, for loops, while loops, etc go here
    /// </summary>
    public class ControlFlowTests
    {
        public static void Run()
        {

            int i2;
            for (i2 = 0; i2 < 100; i2++)
            {
                Console.Write(i2);
                Console.Write(" ");
            }
            if (i2 == 100)
            {
                Console.WriteLine();
                TestController.TestSuccess("For loop test");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Got for loop test result: ");
                Console.WriteLine(i2);

                TestController.TestFail("For loop test");
            }

            var arr = new string[] { "a", "b", "c" };
            foreach (var item in arr)
            {
                if (item == "b")
                {
                    TestController.TestSuccess("ForEach test");
                    break;
                }
                else if (item == "c")
                {
                    TestController.TestFail("Break; does not work in foreach");
                }
            }

            int value = 0;
            TestByRef("this is correct", ref value);
            if (value == 0)
            {
                TestController.TestFail("ByRef value did not change!");
            }
            else if (value == 1)
            {
                TestController.TestSuccess("ByRef value is correct");
            }
            else
            {
                TestController.TestFail("ByRef value is not a 1 or a 0.");
            }
        }

        private static void TestByRef(string t, ref int a)
        {
            Console.WriteLine("Testing ByRef. t = " + t);
            a = 1;
        }
    }
}
