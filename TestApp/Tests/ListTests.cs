using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Tests
{
    public static class ListTests
    {
        public static void Run()
        {
            List<string> list = new List<string>();
            list.Add("item 1");
            list.Add("item 2");
            if (list[0] == "item 1")
            {
                TestController.TestSuccess("Adding item to list");
            }
            else
            {
                TestController.TestFail("Adding item to list failed.");
            }

            TestController.TestAssert(list.Count == 2, "List.Add works");
            list.Clear();

            if (list.Count != 0)
            {
                TestController.TestFail("List count is not zero!");
            }
            else
            {
                TestController.TestSuccess("List count is zero");
            }
        }
    }
}
