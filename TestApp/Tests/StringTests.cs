using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Tests
{
    /// <summary>
    /// Tests related to strings
    /// </summary>
    public static class StringTests
    {
        public static void Run()
        {
            TestController.TestAssert(string.IsNullOrEmpty(null), "string.IsNullOrEmpty(null) works");
            TestController.TestAssert(string.IsNullOrEmpty(""), "string.IsNullOrEmpty(\"\") works");
            TestController.TestAssert(!string.IsNullOrEmpty("a"), "!string.IsNullOrEmpty(\"a\") works");


            //Test strings
            var testString = "TEST";
            if (testString.Length == 4)
            {
                TestController.TestSuccess("String.Length == 4 test");
            }
            else
            {
                TestController.TestFail("String.Length == 4 test fail, testString.Length should be 4, but it is " + testString.Length.ToString());
            }
            if (testString[0] == 'T')
            {
                TestController.TestSuccess("Get char in string test.");
            }
            else
            {
                TestController.TestFail("Get char in string test. Wanted 'T' but got " + testString[0]);
            }
            string superLongString = "Hello there, this is a super duper long string. This is used to see how good my crappy unicode string tabel implementation is";
            Console.WriteLine(superLongString);
            if ('A'.ToString() == "A")
            {
                TestController.TestSuccess("Char.ToString() test");
            }
            else
            {
                TestController.TestFail("Char.ToString() test");
            }

            var str3 = "AB";
            if (str3.IndexOf('B') == 1)
            {
                TestController.TestSuccess("String.IndexOf() is 1");
            }
            else
            {
                TestController.TestFail("String.IndexOf() is not 1");
            }


            var arr = "ABCBE".Split('B');
            TestController.TestAssert(arr[0] == "A", "string.Split works correctly");

        }
    }
}
