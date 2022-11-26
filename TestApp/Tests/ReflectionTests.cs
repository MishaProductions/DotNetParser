using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Tests
{
    public static class ReflectionTests
    {
        public static void Run()
        {
            MyObject obj = new MyObject();
            obj.WelcomeMessage = "Hello!";
            obj.Hello("Hi again!");
            var ret = obj.GetType().FullName;
            if (ret == "TestApp.Tests.MyObject")
            {
                TestController.TestSuccess("obj.GetType().FullName is correct");
            }
            else
            {
                TestController.TestFail("obj.GetType().FullName is incorrect. Expected TestApp.MyObject, but got " + ret);
            }



            var strType = typeof(string);
            var FUllName = strType.FullName;
            if (FUllName == "System.String")
            {
                TestController.TestSuccess("typeof(string).FullName == System.String");
            }
            else
            {
                TestController.TestFail("Full type name of string is System.String, not " + FUllName);
            }
        }
    }
}
