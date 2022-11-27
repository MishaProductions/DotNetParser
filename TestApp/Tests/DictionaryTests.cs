using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Tests
{
    public static class DictionaryTests
    {
        public static void Run()
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            dictionary.Add(2, "Test string #2");

            ////todo: constrained. opcode
            //if (dictionary[2] == "Test string #2")
            //{
            //    TestController.TestSuccess("Dictionary: adding key/value and reading value by key works");
            //}
            //else
            //{
            //    TestController.TestFail("Dictionary: adding key/value and reading value by key doesn't work");
            //}
        }
    }
}
