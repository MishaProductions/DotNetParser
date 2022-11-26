using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Tests
{
    public class ArrayTests
    {
        public static void Run()
        {
            string[] stringArray = new string[8];
            if (stringArray.Length == 8)
            {
               TestController.TestSuccess("Create array");
            }
            else
            {
                TestController.TestFail("Create array, len should be 8 but it is " + stringArray.Length);
            }

            stringArray[0] = "a";
            stringArray[1] = "b";
            if (stringArray[0] == "a")
            {
                TestController.TestSuccess("Read and write to an array");
            }
            else
            {
                TestController.TestFail("Read and write to an array");
            }

            if (Array.Empty<string>().Length == 0)
            {
                TestController.TestSuccess("Array.Empty() returns empty array");
            }
            else
            {
                TestController.TestFail("Array.Empty() returned non empty array");
            }
        }
    }
}
