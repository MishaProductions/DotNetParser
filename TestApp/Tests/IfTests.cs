namespace TestApp.Tests
{
    public static class IfTests
    {
        public static void Run()
        {
            //Equal test
            if (ClrTest() == 90)
            {
                TestController.TestSuccess("Equal Test");
            }
            else
            {
                TestController.TestFail("Equal Test");
            }
            //Inequal test
            if (ClrTest() != 123)
            {
                TestController.TestSuccess("Inequal Test");
            }
            else
            {
                TestController.TestFail("Inequal Test");
            }

            var flag1 = 78 >= 76;
            if (flag1)
            {
                TestController.TestSuccess("78 >= 76");
            }
            else
            {
                TestController.TestFail("78 >= 76 returned false!");
            }
            var flag2 = 78 <= 76;
            if (flag2)
            {
                TestController.TestFail("78 <= 76 returned true!");
            }
            else
            {
                TestController.TestSuccess("78 <= 76");
            }

            var flag3 = false;
            if (!flag3)
            {
                TestController.TestSuccess("NOT test");
            }
            else
            {
                TestController.TestFail("NOT test");
            }
        }

        /// <summary>
        /// Returns 90.
        /// </summary>
        /// <returns>90</returns>
        public static int ClrTest() { return 90; }
    }
}
