using System;

namespace TestApp.Tests
{
    public static class MathTests
    {
        public static void Run()
        {
            //Addition test
            var int2 = IfTests.ClrTest() + 10;
            //Now, int2 should be 100
            if (int2 != 100)
            {
                TestController.TestFail("Add test");
                Console.Write("(3/6) Add test failure! Result: ");
                Console.Write(int2);
                Console.WriteLine();
            }
            else
            {
                TestController.TestSuccess("Add test");
            }
            //Subtraction test
            var int3 = IfTests.ClrTest() - 10;

            //Now, int3 should be 80
            if (int3 != 80)
            {
                TestController.TestFail("Subtract test");
                Console.Write("(4/6) Subtract test failure! Result: ");
                Console.Write(int3);
                Console.WriteLine();
            }
            else
            {
                TestController.TestSuccess("Subtract test");
            }
            //Divide test
            var int4 = IfTests.ClrTest() / 30;
            //Now, int4 should be 3
            if (int4 != 3)
            {
                TestController.TestFail("Divide test");
                Console.Write("(5/6) Divide test failure! Result: ");
                Console.Write(int4);
                Console.WriteLine();
            }
            else
            {
                TestController.TestSuccess("Divide test");
            }
            //Multiply test
            var int5 = IfTests.ClrTest() * 3;
            //Now, int5 should be 3
            if (int5 != 270)
            {
                TestController.TestFail("Multiply test");
                Console.Write("(5/6) Multiply test failure! Result: ");
                Console.Write(int5);
                Console.WriteLine();
            }
            else
            {
                TestController.TestSuccess("Multiply test");
            }
            int int6 = IfTests.ClrTest();
            int6 += 1;

            if (int6 == 91)
            {
                TestController.TestSuccess("Increment test");
            }
            else
            {
                TestController.TestFail("Increment test");
                Console.Write("(6/6) Increment test failure. Result: ");
                Console.Write(int6);
                Console.WriteLine();
            }
            var i = IfTests.ClrTest();
            if (100 > i)
            {
                TestController.TestSuccess("Greater than if statement");
            }
            else
            {
                TestController.TestFail("Greater than if statement");
            }
            if (100 < i)
            {
                TestController.TestFail("Less than if statement");
            }
            else
            {
                TestController.TestSuccess("Less than if statement");
            }

            //
            // Math class tests
            //

            TestController.TestAssert(Math.Clamp(21, 0, 20) == 20, "math clamp returns correct value");
            TestController.TestAssert(Math.Clamp(-1, 0, 20) == 0, "math clamp returns correct value");
            TestController.TestAssert(Math.PI == 3.1415926535897931, "math pi returns correct value");
            TestController.TestAssert(Math.Max(8, 7) == 8, "math max returns correct value");

            //decimal x = 0.1M;
            //if(0.2M > x)
            //{
            //    TestController.TestSuccess("decimal 0.2 > 0.1 is true");
            //}
            //else
            //{
            //    TestController.TestSuccess("decimal 0.2 > 0.1 is FALSE");
            //}
        }
    }
}
